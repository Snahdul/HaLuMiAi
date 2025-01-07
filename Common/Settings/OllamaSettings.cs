namespace Common.Settings;

/// <summary>
/// Represents the settings for Ollama service.
/// </summary>
public class OllamaSettings
{
    /// <summary>
    /// Gets or sets the endpoint URL for the Ollama service.
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the text model identifier for the Ollama service.
    /// </summary>
    public string TextModelId { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the embedding model identifier for the Ollama service.
    /// </summary>
    public string EmbeddingModelId { get; set; } = string.Empty;
}