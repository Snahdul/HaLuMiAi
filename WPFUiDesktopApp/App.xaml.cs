using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Serilog;
using System.Security;
using System.Windows.Threading;
using WPFUiDesktopApp.Views.Windows;

namespace WPFUiDesktopApp;

/// <summary>
/// Interaction logic for App.xaml
/// </summary>
public partial class App
{
    private static readonly IHost AppHost = Hosting.CreateHostBuilder(Environment.GetCommandLineArgs()).Build();

    /// <summary>
    /// Gets or sets the application settings file name.
    /// </summary>
    public static string AppSettingsFileName { get; set; }

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
    }

    /// <summary>
    /// Handles unhandled exceptions in the current application domain.
    /// </summary>
    /// <param name="sender">The source of the unhandled exception event.</param>
    /// <param name="e">The <see cref="UnhandledExceptionEventArgs"/> instance containing the event data.</param>
    internal static void CurrentDomainUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        Log.Fatal(e.ExceptionObject is Exception exception ? exception : new Exception("Unknown exception"),
            "An unhandled exception occurred");
    }

    /// <summary>
    /// Displays the main window of the application.
    /// </summary>
    internal static void DisplayMainWindow()
    {
        if (!Application.Current.Windows.OfType<MainWindow>().Any())
        {
            var mainWindow = GetRequiredService<MainWindow>();
            mainWindow.Show();
        }
    }

    /// <summary>
    /// Occurs when the application is closing.
    /// </summary>
    /// <inheritdoc />
    protected override void OnExit(ExitEventArgs e)
    {
        // Call base method
        base.OnExit(e);

        // Run the asynchronous operation asynchronously
        try
        {
            AppHost.StopAsync(TimeSpan.FromSeconds(5)).GetAwaiter().GetResult();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "An error occurred while stopping the application host");
        }
        finally
        {
            AppHost.Dispose();
        }
    }

    #endregion

    /// <summary>
    /// Handles unhandled exceptions in the dispatcher.
    /// </summary>
    /// <param name="sender">The source of the unhandled exception event.</param>
    /// <param name="e">The <see cref="DispatcherUnhandledExceptionEventArgs"/> instance containing the event data.</param>
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

        // Prevent default unhandled exception processing
        e.Handled = true;
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
}