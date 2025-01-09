using Microsoft.Extensions.Logging;
using Microsoft.KernelMemory;
using Microsoft.KernelMemory.DataFormats;
using Microsoft.KernelMemory.DataFormats.WebPages;
using Microsoft.KernelMemory.Diagnostics;
using Microsoft.KernelMemory.Pipeline;
using NTextCat;
using System.Text;
using System.Text.Json;

namespace HaMiAi.Implementation.Handler;

/// <summary>
/// Memory ingestion pipeline handler responsible for extracting German text from files and saving it to document storage.
/// </summary>
public sealed class GermanTextExtractionHandler : IPipelineStepHandler, IDisposable
{
    private readonly IPipelineOrchestrator _orchestrator;
    private readonly IEnumerable<IContentDecoder> _decoders;
    private readonly IWebScraper _webScraper;
    private readonly ILogger<GermanTextExtractionHandler> _log;
    private readonly RankedLanguageIdentifier _identifier;

    /// <inheritdoc />
    public string StepName { get; }

    /// <summary>
    /// Handler responsible for extracting German text from documents.
    /// Note: stepName and other params are injected with DI.
    /// </summary>
    /// <param name="stepName">Pipeline step for which the handler will be invoked</param>
    /// <param name="orchestrator">Current orchestrator used by the pipeline, giving access to content and other helps.</param>
    /// <param name="decoders">The list of content decoders for extracting content</param>
    /// <param name="webScraper">Web scraper instance used to fetch web pages</param>
    /// <param name="loggerFactory">Application logger factory</param>
    public GermanTextExtractionHandler(
        string stepName,
        IPipelineOrchestrator orchestrator,
        IEnumerable<IContentDecoder> decoders,
        IWebScraper? webScraper = null,
        ILoggerFactory? loggerFactory = null)
    {
        this.StepName = stepName;
        this._orchestrator = orchestrator;
        this._decoders = decoders;
        this._log = (loggerFactory ?? DefaultLogger.Factory).CreateLogger<GermanTextExtractionHandler>();
        this._webScraper = webScraper ?? new WebScraper();

        var factory = new RankedLanguageIdentifierFactory();
        this._identifier = factory.Load("LanguageModels\\Core14.profile.xml"); // Ensure the profile is available

        this._log.LogInformation("Handler '{0}' ready", stepName);
    }

    /// <inheritdoc />
    public async Task<(ReturnType returnType, DataPipeline updatedPipeline)> InvokeAsync(
        DataPipeline pipeline, CancellationToken cancellationToken = default)
    {
        this._log.LogDebug("Extracting German text, pipeline '{0}/{1}'", pipeline.Index, pipeline.DocumentId);

        foreach (DataPipeline.FileDetails uploadedFile in pipeline.Files)
        {
            if (uploadedFile.AlreadyProcessedBy(this))
            {
                this._log.LogTrace("File {0} already processed by this handler", uploadedFile.Name);
                continue;
            }

            var sourceFile = uploadedFile.Name;
            var destFile = $"{uploadedFile.Name}.extract.de.txt";
            BinaryData fileContent = await this._orchestrator.ReadFileAsync(pipeline, sourceFile, cancellationToken).ConfigureAwait(false);

            string text = string.Empty;
            FileContent content = new(MimeTypes.PlainText);
            bool skipFile = false;

            if (fileContent.ToArray().Length > 0)
            {
                if (uploadedFile.MimeType == MimeTypes.WebPageUrl)
                {
                    var (downloadedPage, pageContent, skip) = await this.DownloadContentAsync(uploadedFile, fileContent, cancellationToken).ConfigureAwait(false);
                    skipFile = skip;
                    if (!skipFile)
                    {
                        (text, content, skipFile) = await this.ExtractTextAsync(downloadedPage, pageContent, cancellationToken).ConfigureAwait(false);
                    }
                }
                else
                {
                    (text, content, skipFile) = await this.ExtractTextAsync(uploadedFile, fileContent, cancellationToken).ConfigureAwait(false);
                }
            }

            // If the handler cannot extract text, we move on. There might be other handlers in the pipeline
            // capable of doing so, and in any case if a document contains multiple docs, the pipeline will
            // not fail, only do its best to export as much data as possible. The user can inspect the pipeline
            // status to know if a file has been ignored.
            if (!skipFile)
            {
                // Text file
                this._log.LogDebug("Saving extracted German text file {0}", destFile);
                await this._orchestrator.WriteFileAsync(pipeline, destFile, new BinaryData(text), cancellationToken).ConfigureAwait(false);
                var destFileDetails = new DataPipeline.GeneratedFileDetails
                {
                    Id = Guid.NewGuid().ToString("N"),
                    ParentId = uploadedFile.Id,
                    Name = destFile,
                    Size = text.Length,
                    MimeType = content.MimeType,
                    ArtifactType = DataPipeline.ArtifactTypes.ExtractedText,
                    Tags = pipeline.Tags,
                };
                destFileDetails.MarkProcessedBy(this);
                uploadedFile.GeneratedFiles.Add(destFile, destFileDetails);
            }

            uploadedFile.MarkProcessedBy(this);
        }

        return (ReturnType.Success, pipeline);
    }

    public void Dispose()
    {
        if (this._webScraper is not IDisposable x) { return; }

        x.Dispose();
    }

    private async Task<(DataPipeline.FileDetails downloadedPage, BinaryData pageContent, bool skip)> DownloadContentAsync(
        DataPipeline.FileDetails uploadedFile, BinaryData fileContent, CancellationToken cancellationToken)
    {
        var url = fileContent.ToString();
        this._log.LogDebug("Downloading web page specified in '{0}' and extracting text from '{1}'", uploadedFile.Name, url);
        if (string.IsNullOrWhiteSpace(url))
        {
            uploadedFile.Log(this, "The web page URL is empty");
            this._log.LogWarning("The web page URL is empty");
            return (uploadedFile, fileContent, skip: true);
        }

        var urlDownloadResult = await this._webScraper.GetContentAsync(url, cancellationToken).ConfigureAwait(false);
        if (!urlDownloadResult.Success)
        {
            uploadedFile.Log(this, $"Web page download error: {urlDownloadResult.Error}");
            this._log.LogWarning("Web page download error: {0}", urlDownloadResult.Error);
            return (uploadedFile, fileContent, skip: true);
        }

        if (urlDownloadResult.Content.Length == 0)
        {
            uploadedFile.Log(this, "The web page has no text content, skipping it");
            this._log.LogWarning("The web page has no text content, skipping it");
            return (uploadedFile, fileContent, skip: true);
        }

        // IMPORTANT: copy by value to avoid editing the source var
        DataPipeline.FileDetails? result = JsonSerializer.Deserialize<DataPipeline.FileDetails>(JsonSerializer.Serialize(uploadedFile));
        ArgumentNullExceptionEx.ThrowIfNull(result, nameof(result), "File details cloning failure");

        result.MimeType = urlDownloadResult.ContentType;
        result.Size = urlDownloadResult.Content.Length;

        return (result, urlDownloadResult.Content, skip: false);
    }

    private async Task<(string text, FileContent content, bool skipFile)> ExtractTextAsync(
        DataPipeline.FileDetails uploadedFile,
        BinaryData fileContent,
        CancellationToken cancellationToken)
    {
        // Define default empty content
        var content = new FileContent(MimeTypes.PlainText);

        if (string.IsNullOrEmpty(uploadedFile.MimeType))
        {
            uploadedFile.Log(this, $"File MIME type is empty, ignoring the file {uploadedFile.Name}");
            this._log.LogWarning("Empty MIME type, file '{0}' will be ignored", uploadedFile.Name);
            return (text: string.Empty, content, skipFile: true);
        }

        // Checks if there is a decoder that supports the file MIME type. If multiple decoders support this type, it means that
        // the decoder has been redefined, so it takes the last one.
        var decoder = this._decoders.LastOrDefault(d => d.SupportsMimeType(uploadedFile.MimeType));
        if (decoder is not null)
        {
            this._log.LogDebug("Extracting text from file '{0}' mime type '{1}' using extractor '{2}'",
                uploadedFile.Name, uploadedFile.MimeType, decoder.GetType().FullName);
            content = await decoder.DecodeAsync(fileContent, cancellationToken).ConfigureAwait(false);
        }
        else
        {
            uploadedFile.Log(this, $"File MIME type not supported: {uploadedFile.MimeType}. Ignoring the file {uploadedFile.Name}.");
            this._log.LogWarning("File MIME type not supported: {0} - ignoring the file {1}", uploadedFile.MimeType, uploadedFile.Name);
            return (text: string.Empty, content, skipFile: true);
        }

        var textBuilder = new StringBuilder();
        foreach (var section in content.Sections)
        {
            var sectionContent = section.Content.Trim();
            if (string.IsNullOrEmpty(sectionContent)) { continue; }

            var germanText = ExtractGermanText(sectionContent);
            if (!string.IsNullOrEmpty(germanText))
            {
                textBuilder.Append(germanText);
                textBuilder.AppendLine();
            }
        }

        var text = textBuilder.ToString().Trim();

        return (text, content, skipFile: false);
    }

    private string ExtractGermanText(string content)
    {
        var germanText = new StringBuilder();
        var sentences = content.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (var sentence in sentences)
        {
            var trimmedSentence = sentence.Trim();
            if (!string.IsNullOrEmpty(trimmedSentence))
            {
                var languages = _identifier.Identify(trimmedSentence);
                var mostLikelyLanguage = languages.FirstOrDefault();

                if (mostLikelyLanguage != null && mostLikelyLanguage.Item1.Iso639_3 == "deu")
                {
                    germanText.AppendLine(trimmedSentence);
                }
            }
        }

        return germanText.ToString();
    }
}