using Autofac;
using Autofac.Extensions.DependencyInjection;
using ChatConversationControl.Contracts;
using ChatConversationControl.Implementation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using Wpf.Ui;
using WPFUiDesktopApp.Services;
using WPFUiDesktopApp.ViewModels.Pages;
using WPFUiDesktopApp.ViewModels.Windows;
using WPFUiDesktopApp.Views.Pages;
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
            .ConfigureContainer<ContainerBuilder>(builder =>
            {
                //builder.RegisterModule<HaMiAIModule>();
            })
            .ConfigureAppConfiguration((context, config) =>
            {
                var env = context.HostingEnvironment;
                config.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true, reloadOnChange: true)
                    .AddEnvironmentVariables();
            })
            .ConfigureServices((hostContext, services) =>
            {
                var configuration = hostContext.Configuration;

                // Add options services and configure AppSettings with validation
                //services.AddOptions<AppSettings>()
                //    .Bind(configuration.GetSection("AppSettings"))
                //    .Validate(appSettings => !string.IsNullOrEmpty(appSettings?.OllamaSettings?.Endpoint), "OllamaSettings.Endpoint must not be null or empty.");

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

                services.AddSingleton<DashboardPage>();
                services.AddSingleton<DashboardViewModel>();
                services.AddSingleton<SettingsPage>();
                services.AddSingleton<SettingsViewModel>();

                //services.AddSingleton<OllamaChatViewModel>();
                //services.AddSingleton<OllamaPage>();

                //services.AddSingleton<OllamaMemoryViewModel>();

                //services.AddSingleton<WebpageImportDialogViewModel>();

                //services.AddTransient<ConversationControlViewModel>();

                //services.AddTransient<OllamaViewModel>();
                //services.AddTransient<OllamaMemoryModel>();

                //services.AddSingleton<ViewModels.UserControls.MemoryConversationControlViewModel>();

                //services.AddSingleton<AddFileToMemoryViewModel>();
                //services.AddSingleton<AddWebpageToMemoryViewModel>();

                //services.AddTransient<TagManagerViewModel>();
                //services.AddTransient<StorageManagementViewModel>();
            }).UseSerilog((context, services, configuration) => configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .WriteTo.Console()
                .WriteTo.File("hami/log/log.txt", rollingInterval: RollingInterval.Day));
}