using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Boxes.Models;
using Boxes.Windows;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using WinRT;

namespace Boxes;

/// <summary>
/// Simple launcher for testing desktop boxes
/// </summary>
public static class BoxLauncher
{
    public static void Main(string[] args)
    {
        try
        {
            // Write to console for debugging
            Console.WriteLine("BoxLauncher starting...");
            
            // Initialize COM
            WinRT.ComWrappersSupport.InitializeComWrappers();
            Console.WriteLine("COM initialized");

            // Create a sample box
            var sampleBox = new Box
            {
                Id = Guid.NewGuid(),
                Name = "Sample Desktop Box",
                Description = "A beautiful glass-like desktop container",
                Style = BoxStyle.Acetate,
                X = 100,
                Y = 100,
                Width = 400,
                Height = 300,
                IsVisible = true,
                CreatedDate = DateTime.Now
            };
            Console.WriteLine($"Sample box created: {sampleBox.Name}");

            // Start the WinUI application
            Console.WriteLine("Starting WinUI application...");
            Microsoft.UI.Xaml.Application.Start((p) =>
            {
                Console.WriteLine("WinUI application started");
                var context = new Microsoft.UI.Dispatching.DispatcherQueueSynchronizationContext(
                    Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread());
                System.Threading.SynchronizationContext.SetSynchronizationContext(context);

                Console.WriteLine("Creating simple window...");
                // Create a simple window first to test
                var testWindow = new Microsoft.UI.Xaml.Window();
                testWindow.Title = "Test Window";
                testWindow.Activate();
                Console.WriteLine("Test window activated");
            });
        }
        catch (Exception ex)
        {
            // Write to console for debugging
            Console.WriteLine($"BoxLauncher Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            
            // Log error to debug output
            System.Diagnostics.Debug.WriteLine($"BoxLauncher Error: {ex.Message}");
            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            
            // Keep console open to see error
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
