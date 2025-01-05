using Autofac;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Windows.Threading;
using Wpf.Ui;
using WPFUiDesktopApp.Views.Windows;

namespace WPFUiDesktopApp
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        private static readonly IHost AppHost = CreateHostBuilder().Build();

        /// <summary>
        /// Retrieves a service of type <typeparamref name="T"/> from the application's service provider.
        /// </summary>
        /// <typeparam name="T">The type of the service object to get.</typeparam>
        /// <returns>A service object of type <typeparamref name="T"/>.</returns>
        /// <exception cref="InvalidOperationException">Thrown when the service of type <typeparamref name="T"/> cannot be found.</exception>
        /// <remarks>
        /// This method is a convenience wrapper around <see> <cref>IServiceProvider.GetRequiredService(Type)</cref></see>
        /// providing a strongly-typed way to retrieve a service instance. It simplifies the process of fetching
        /// services from the application's service container, ensuring that a valid instance is returned or
        /// an exception is thrown if the service cannot be resolved. This ensures that services are correctly
        /// registered and available when requested, aiding in the detection of configuration issues during development.
        /// </remarks>
        public static T GetRequiredService<T>() where T : class => AppHost.Services.GetRequiredService<T>();

        #region Overrides of Application

        /// <summary>
        /// Occurs when the application is loading.
        /// </summary>
        /// <inheritdoc />
        /// <exception cref="SecurityException">The caller does not have the required permission to perform this operation.</exception>
        /// <exception cref="ArgumentException"><paramref name="startupEventArgs" /> contains a zero-length string, an initial hexadecimal zero character (0x00), or an equal sign ("=").
        ///  -or-
        ///  The length of <paramref name="startupEventArgs" /> or <paramref name="startupEventArgs" /> is greater than or equal to 32,767 characters.
        ///  -or-
        ///  An error occurred during the execution of this operation.</exception>
        protected override void OnStartup(StartupEventArgs startupEventArgs)
        {
            base.OnStartup(startupEventArgs);

            // Set the environment based on command line arguments
            var eArgs = startupEventArgs.Args;
            SetEnvironment(ref eArgs);

            AppDomain.CurrentDomain.UnhandledException += CurrentDomainUnhandledException;

            AppHost.Start();

            DisplayMainWindow();
            return;
        }

        internal static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            Log.Fatal(e.ExceptionObject is Exception exception ? exception : new Exception("Unknown exception"),
                "An unhandled exception occurred");
        }

        internal static void DisplayMainWindow()
        {
            if (!Application.Current.Windows.OfType<MainWindow>().Any())
            {
                var mainWindow = App.GetRequiredService<MainWindow>();
                mainWindow.Show();
            }
        }

        /// <summary>
        /// Occurs when the application is closing.
        /// </summary>
        /// <inheritdoc />
        /// <exception cref="NullReferenceException">The <see cref="TaskAwaiter" /> object was not properly initialized.</exception>
        /// <exception cref="TaskCanceledException">The task was canceled.</exception>
        /// <exception cref="Exception">The task completed in a <see cref="System.Threading.Tasks.TaskStatus.Faulted" /> state.</exception>
        protected override void OnExit(ExitEventArgs e)
        {
            // Call base method
            base.OnExit(e);

            // Run the asynchronous operation synchronously
            // ReSharper disable once ExceptionNotDocumented
            // ReSharper disable once ExceptionNotDocumentedOptional
            AppHost.StopAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();

            AppHost.Dispose();
        }

        #endregion

        /// <summary>
        /// Creates and configures the host builder for the application.
        /// </summary>
        /// <returns>The configured host builder.</returns>
        // The.NET Generic Host provides dependency injection, configuration, logging, and other services.
        // https://docs.microsoft.com/dotnet/core/extensions/generic-host
        // https://docs.microsoft.com/dotnet/core/extensions/dependency-injection
        // https://docs.microsoft.com/dotnet/core/extensions/configuration
        // https://docs.microsoft.com/dotnet/core/extensions/logging
        private static IHostBuilder CreateHostBuilder()
        {


            var hostConfiguration = Hosting.CreateHostBuilder();

            hostConfiguration.ConfigureContainer<ContainerBuilder>((context, containerBuilder) =>
            {
                RegisterViewModels(containerBuilder);
                RegisterViews(containerBuilder);
            });

            hostConfiguration.ConfigureServices((context, services) =>
            {
                services.AddSingleton<IContentDialogService, ContentDialogService>();
            });

            return hostConfiguration;
        }

        private static void RegisterViewModels(ContainerBuilder containerBuilder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var viewModelTypes = assembly.GetTypes()
                .Where(type => type.Namespace == "UiDesktopApp.ViewModels" &&
                               type.IsAssignableTo(typeof(ObservableObject)))
                .ToArray();

            foreach (var type in viewModelTypes)
            {
                var registration = containerBuilder.RegisterType(type)
                    .AsSelf()
                    .InstancePerLifetimeScope().AsImplementedInterfaces();

                if (typeof(IStartable).IsAssignableFrom(type))
                {
                    registration.As<IStartable>();
                }
            }
        }

        private static void RegisterViews(ContainerBuilder containerBuilder)
        {
            var assembly = Assembly.GetExecutingAssembly();

            var viewTypes = assembly.GetTypes()
                .Where(type => type is { Namespace: "UiDesktopApp.Views", IsClass: true, IsAbstract: false })
                .ToArray();

            foreach (var type in viewTypes)
            {
                containerBuilder.RegisterType(type).AsSelf().SingleInstance();
            }
        }

        /// <summary>
        /// Occurs when an exception is thrown by an application but not handled.
        /// </summary>
        private void OnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            // For more info see https://docs.microsoft.com/en-us/dotnet/api/system.windows.application.dispatcherunhandledexception?view=windowsdesktop-6.0
            if (e.Exception == null)
            {
                Log.Fatal("An unhandled exception of unknown type occurred");
            }
            else
            {
                Log.Fatal(e.Exception, "An unhandled exception occurred");
            }
        }

        /// <summary>
        /// Sets the environment based on the command line arguments.
        /// </summary>
        /// <param name="args">Command line arguments.</param>
        /// <exception cref="ArgumentNullException"><paramref name="args" /> is <see langword="null" />.</exception>
        /// <exception cref="SecurityException">The caller does not have the required permission to perform this operation.</exception>
        /// <exception cref="ArgumentException"><paramref name="args" /> contains a zero-length string, an initial hexadecimal zero character (0x00), or an equal sign ("=").
        ///  -or-
        ///  The length of <paramref name="args" /> or <paramref name="args" /> is greater than or equal to 32,767 characters.
        ///  -or-
        ///  An error occurred during the execution of this operation.</exception>
        internal static void SetEnvironment(ref string[] args)
        {
            const string environmentOption = "--environment";
            var environmentArgIndex = Array.IndexOf(args, environmentOption);

            if (environmentArgIndex < 0 || args.Length <= environmentArgIndex + 1)
            {
                return;
            }

            var environmentName = args[environmentArgIndex + 1];

            // Remove the environment argument from the args array
            args = args.Where((arg, index) => index != environmentArgIndex && index != environmentArgIndex + 1).ToArray();

            if (string.IsNullOrEmpty(environmentName))
            {
                return;
            }

            Log.Information($"Setting environment to {environmentName}");
            Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", environmentName);
        }

        /// <summary>
        /// Parses the environment argument from the command line arguments.
        /// </summary>
        /// <param name="args">The command line arguments.</param>
        /// <returns>The environment name if found; otherwise, null.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="args" /> is <see langword="null" />.</exception>
        private static string? ParseEnvironmentArgument(ref string[] args)
        {
            const string environmentOption = "--environment";
            var environmentArgIndex = Array.IndexOf(args, environmentOption);

            if (environmentArgIndex < 0 || args.Length <= environmentArgIndex + 1) return default;

            var environmentName = args[environmentArgIndex + 1];
            // Remove the environment argument from the args array
            args = args.Where((arg, index) => index != environmentArgIndex && index != environmentArgIndex + 1).ToArray();
            return environmentName;
        }
    }
}
