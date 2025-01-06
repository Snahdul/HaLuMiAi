using Autofac;
using CommunityToolkit.Diagnostics;
using Microsoft.Extensions.AI;
using System.Diagnostics.CodeAnalysis;

namespace HaMiAI;

/// <summary>
/// Autofac module for registering services related to Semantic Kernel.
/// </summary>
[ExcludeFromCodeCoverage]
public class HaMiAIModule : Module
{
    private readonly Uri _endpointUri;
    private readonly string _modelId;

    /// <summary>
    /// Initializes a new instance of the <see cref="HaMiAIModule"/> class.
    /// </summary>
    /// <param name="endpointUri">The endpoint URI for the OllamaChatClient.</param>
    /// <param name="modelId">The model ID for the OllamaChatClient.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="endpointUri" /> or <paramref name="modelId" /> is <see langword="null" />.</exception>
    public HaMiAIModule(Uri endpointUri, string modelId)
    {
        Guard.IsNotNull(endpointUri);
        Guard.IsNotNullOrWhiteSpace(modelId);

        _endpointUri = endpointUri;
        _modelId = modelId;
    }

    /// <summary>
    /// Override to add registrations to the container.
    /// </summary>
    /// <param name="builder">The builder through which components can be registered.</param>
    protected override void Load(ContainerBuilder builder)
    {
        RegisterOllamaChatClient(builder);
    }

    /// <summary>
    /// Registers the OllamaChatClient with the provided settings.
    /// </summary>
    /// <param name="builder">The builder through which components can be registered.</param>
    private void RegisterOllamaChatClient(ContainerBuilder builder)
    {
        // Register OllamaChatClient using the provided settings
        builder.RegisterInstance(new OllamaChatClient(endpoint: _endpointUri, modelId: _modelId))
            .As<IChatClient>()
            .SingleInstance();
    }
}