using Autofac;
using Common.Settings;
using CommunityToolkit.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace OllamaKernelMemory;

/// <summary>
/// Autofac module for registering services related to Ollama kernel memory.
/// </summary>
[ExcludeFromCodeCoverage]
public class OllamaKernelMemoryModule : Module
{
    private readonly OllamaSettings _ollamaSettings;

    /// <summary>
    /// Initializes a new instance of the <see cref="OllamaKernelMemoryModule"/> class.
    /// </summary>
    /// <param name="ollamaSettings">The settings for the Ollama service.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="ollamaSettings" /> is <see langword="null" />.</exception>
    public OllamaKernelMemoryModule(OllamaSettings ollamaSettings)
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
        RegisterOllamaKernelMemory(builder);
    }

    private void RegisterOllamaKernelMemory(ContainerBuilder builder)
    {
        builder.RegisterType<OllamaKernelMemoryQueryService>().AsImplementedInterfaces().SingleInstance();
    }
}