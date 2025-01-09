using Autofac;
using Autofac.Extensions.DependencyInjection;
using ChatConversationControl.Contracts;
using ChatConversationControl.Implementation;
using Common.Settings;
using HaMiAi;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.IO.Abstractions;
using System.Reflection;
using Wpf.Ui;
using WPFUiDesktopApp.Models;
using WPFUiDesktopApp.Services;
using WPFUiDesktopApp.Settings;
using WPFUiDesktopApp.Views.Windows;

namespace WPFUiDesktopApp;

/// <summary>
/// Provides methods to create and configure the host for the application.
/// </summary>
internal class Hosting
{
    /// <summary>
    /// Creates and configures an <see cref="IHostBuilder"/> instance.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    /// <returns>An <see cref="IHostBuilder"/> instance.</returns>
    internal static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>((context, builder) =>
            {
                var configuration = context.Configuration;
                var appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();
                if (appSettings is null)
                {
                    throw new InvalidOperationException("AppSettings must not be null.");
                }

                builder.RegisterModule(new HaMiAIModule(appSettings.OllamaSettings));

                // Register IFileSystem with its implementation
                builder.RegisterType<FileSystem>().As<IFileSystem>().SingleInstance();

                // Register view models and views
                RegisterViewModels(builder);
                RegisterViews(builder);

                // Register custom registration source for logging missing registrations
                builder.RegisterSource(new MissingRegistrationLogger());
            })
            .ConfigureAppConfiguration((context, config) =>
            {
                var env = context.HostingEnvironment;
                var configFileName = $"appsettings.{env.EnvironmentName}.json";

                config.AddJsonFile(configFileName, optional: false, reloadOnChange: false)
                      .AddEnvironmentVariables();

                App.AppSettingsFileName = configFileName;
            })
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;

                // Add options services and configure AppSettings with validation
                services.AddOptions<AppSettings>()
                    .Bind(configuration.GetSection("AppSettings"))
                    .Validate(appSettings => !string.IsNullOrEmpty(appSettings?.OllamaSettings?.Endpoint), "OllamaSettings.Endpoint must not be null or empty.");

                // Register IOptions<OllamaSettings>
                services.Configure<OllamaSettings>(configuration.GetSection("AppSettings:OllamaSettings"));

                services.AddHostedService<ApplicationHostService>();

                // Page resolver service
                services.AddSingleton<IPageService, PageService>();

                // Theme manipulation
                services.AddSingleton<IThemeService, ThemeService>();

                // TaskBar manipulation
                services.AddSingleton<ITaskBarService, TaskBarService>();

                services.AddSingleton<SettingsService>();

                services.AddSingleton<IContentDialogService, ContentDialogService>();

                // Service containing navigation, same as INavigationWindow... but without window
                services.AddSingleton<INavigationService, NavigationService>();

                services.AddSingleton<IConversationManager, ConversationManagerDefault>();

                // Main window with navigation
                services.AddSingleton<INavigationWindow, MainWindow>();

                services.AddSingleton<IFileDialogService, FileDialogService>();

                services.AddSingleton<OllamaMemoryModel>();
            }).UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .WriteTo.Console()
                .WriteTo.File("hami/log/log.txt", rollingInterval: RollingInterval.Day));

    /// <summary>
    /// Registers the view models in the Autofac container.
    /// </summary>
    /// <param name="containerBuilder">The Autofac container builder.</param>
    private static void RegisterViewModels(ContainerBuilder containerBuilder)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var viewModelTypes = assembly.GetTypes()
            .Where(type => type.Namespace != null && type.Namespace.StartsWith("WPFUiDesktopApp.ViewModels") &&
                           type.IsAssignableTo(typeof(ObservableObject)))
            .ToArray();

        foreach (var type in viewModelTypes)
        {
            var registration = containerBuilder.RegisterType(type)
                .AsSelf()
                .SingleInstance();

            if (typeof(IStartable).IsAssignableFrom(type))
            {
                registration.As<IStartable>();
            }
        }
    }

    /// <summary>
    /// Registers the views in the Autofac container.
    /// </summary>
    /// <param name="containerBuilder">The Autofac container builder.</param>
    private static void RegisterViews(ContainerBuilder containerBuilder)
    {
        var assembly = Assembly.GetExecutingAssembly();

        var viewTypes = assembly.GetTypes()
            .Where(type => type.Namespace != null && type.Namespace.StartsWith("WPFUiDesktopApp.Views"))
            .ToArray();

        foreach (var type in viewTypes)
        {
            containerBuilder.RegisterType(type).AsSelf().SingleInstance();
        }
    }
}