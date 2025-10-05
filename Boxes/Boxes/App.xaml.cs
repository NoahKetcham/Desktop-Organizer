using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using Boxes.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;

namespace Boxes;

/// <summary>
/// Provides application-specific behavior to supplement the default Application class.
/// </summary>
public partial class App : Application
{
    /// <summary>
    /// Gets the current <see cref="App"/> instance in use
    /// </summary>
    public new static App Current => (App)Application.Current;

    /// <summary>
    /// Gets the main window instance
    /// </summary>
    public Window? Window { get; private set; }

    /// <summary>
    /// Gets the <see cref="IServiceProvider"/> instance to resolve application services.
    /// </summary>
    public IServiceProvider Services { get; }

    /// <summary>
    /// Initializes the singleton application object.  This is the first line of authored code
    /// executed, and as such is the logical equivalent of main() or WinMain().
    /// </summary>
    public App()
    {
        // Ensure the host application is running
        EnsureHostIsRunning();

        InitializeComponent();

        // Configure dependency injection
        Services = ConfigureServices();
    }

    /// <summary>
    /// Invoked when the application is launched.
    /// </summary>
    /// <param name="args">Details about the launch request and process.</param>
    protected override void OnLaunched(Microsoft.UI.Xaml.LaunchActivatedEventArgs args)
    {
        Window = new MainWindow();
        Window.Activate();
    }

    /// <summary>
    /// Configures the services for the application.
    /// </summary>
    private static IServiceProvider ConfigureServices()
    {
        var services = new ServiceCollection();

        // ViewModels
        services.AddTransient<MainViewModel>();
        services.AddTransient<TemplatesViewModel>();
        services.AddTransient<OrganizationMethodsViewModel>();
        services.AddTransient<DesktopFilesViewModel>();
        services.AddTransient<BoxesViewModel>();

        // Services
        services.AddSingleton<Services.FileScanner>();
        services.AddSingleton<Services.RuleEngine>();
        services.AddSingleton<Services.BoxManager>();

        return services.BuildServiceProvider();
    }

    /// <summary>
    /// Ensures that the Boxes.Host application is running in the background.
    /// </summary>
    private static void EnsureHostIsRunning()
    {
        try
        {
            // Check if Boxes.Host.exe is already running
            var existingProcesses = System.Diagnostics.Process.GetProcessesByName("Boxes.Host");
            if (existingProcesses.Length > 0)
            {
                Debug.WriteLine("EnsureHostIsRunning: Boxes.Host is already running");
                return;
            }

            Debug.WriteLine("EnsureHostIsRunning: Attempting to start Boxes.Host...");

            // Try to find and start the host executable
            string? hostExePath = @"C:\Users\noahk\OneDrive\Documents\GitHub\Desktop-Organizer\Boxes.Host\x64\Debug\Boxes.Host.exe";

            if (File.Exists(hostExePath))
            {
                Debug.WriteLine($"EnsureHostIsRunning: Starting Boxes.Host from: {hostExePath}");

                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = hostExePath,
                    UseShellExecute = true,
                    CreateNoWindow = true
                };

                System.Diagnostics.Process.Start(startInfo);
                Debug.WriteLine("EnsureHostIsRunning: Boxes.Host started successfully");
            }
            else
            {
                Debug.WriteLine($"EnsureHostIsRunning: Boxes.Host.exe not found at: {hostExePath}");
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"EnsureHostIsRunning: Failed to start Boxes.Host: {ex.Message}");
        }
    }
}
