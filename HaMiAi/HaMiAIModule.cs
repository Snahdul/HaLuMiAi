using Autofac;
using Common.Settings;
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
    private readonly OllamaSettings _ollamaSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="HaMiAIModule"/> class.
    /// </summary>
    /// <param name="ollamaSettings">The settings for the Ollama service.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="ollamaSettings" /> is <see langword="null" />.</exception>
    public HaMiAIModule(OllamaSettings ollamaSettings)
    {
        _ollamaSettings = ollamaSettings;
        Guard.IsNotNull(ollamaSettings);
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
        builder.RegisterInstance(
                new OllamaChatClient(
                    endpoint: _ollamaSettings.Endpoint,
                    modelId: _ollamaSettings.TextModelId))
            .As<IChatClient>()
            .SingleInstance();
    }
}