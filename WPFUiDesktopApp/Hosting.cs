using Autofac;
using Autofac.Extensions.DependencyInjection;
using ChatConversationControl.Contracts;
using ChatConversationControl.Implementation;
using HaMiAI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.IO.Abstractions;
using Wpf.Ui;
using WPFUiDesktopApp.Services;
using WPFUiDesktopApp.Settings;
using WPFUiDesktopApp.ViewModels.Windows;
using WPFUiDesktopApp.Views.Windows;

namespace WPFUiDesktopApp;

internal class Hosting
{
    /// <summary>
    /// Creates and configures an <see cref="IHostBuilder"/> instance.
    /// </summary>
    /// <param name="args">The command-line arguments.</param>
    /// <returns>An <see cref="IHostBuilder"/> instance.</returns>
    internal static IHostBuilder CreateHostBuilder() =>
        Host.CreateDefaultBuilder()
            .UseServiceProviderFactory(new AutofacServiceProviderFactory())
            .ConfigureContainer<ContainerBuilder>((context, builder) =>
            {
                var configuration = context.Configuration;
                var appSettings = configuration.GetSection("AppSettings").Get<AppSettings>();
                if (appSettings is null)
                {
                    throw new InvalidOperationException("AppSettings must not be null.");
                }

                var endpointUri = new Uri(appSettings.OllamaSettings.Endpoint);
                var modelId = appSettings.OllamaSettings.TextModelId;

                builder.RegisterModule(new HaMiAIModule(endpointUri, modelId));

                // Register IFileSystem with its implementation
                builder.RegisterType<FileSystem>().As<IFileSystem>().SingleInstance();
            })
            .ConfigureAppConfiguration((context, config) =>
            {
                var env = context.HostingEnvironment;
                App.AppSettingsFileName = $"appsettings.{env.EnvironmentName}.json";

                config.AddJsonFile(App.AppSettingsFileName, optional: false, reloadOnChange: true)
                      .AddEnvironmentVariables();
            })
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;

                // Add options services and configure AppSettings with validation
                services.AddOptions<AppSettings>()
                    .Bind(configuration.GetSection("AppSettings"))
                    .Validate(appSettings => !string.IsNullOrEmpty(appSettings?.OllamaSettings?.Endpoint), "OllamaSettings.Endpoint must not be null or empty.");

                services.AddHostedService<ApplicationHostService>();

                // Page resolver service
                services.AddSingleton<IPageService, PageService>();

                // Theme manipulation
                services.AddSingleton<IThemeService, ThemeService>();

                // TaskBar manipulation
                services.AddSingleton<ITaskBarService, TaskBarService>();

                // Service containing navigation, same as INavigationWindow... but without window
                services.AddSingleton<INavigationService, NavigationService>();

                services.AddSingleton<IConversationManager, ConversationManagerDefault>();

                // Main window with navigation
                services.AddSingleton<INavigationWindow, MainWindow>();
                services.AddSingleton<MainWindowViewModel>();

                services.AddSingleton<IFileDialogService, FileDialogService>();
            }).UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .WriteTo.Console()
                .WriteTo.File("hami/log/log.txt", rollingInterval: RollingInterval.Day));
}
