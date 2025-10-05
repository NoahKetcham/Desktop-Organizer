using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Boxes.Avalonia.Views;
using Boxes.Models;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Boxes.Avalonia;

public static class BoxLauncher
{
    private static ObservableCollection<DesktopBox> _activeBoxes = new();

    /// <summary>
    /// Gets the collection of currently active desktop boxes
    /// </summary>
    public static ObservableCollection<DesktopBox> ActiveBoxes => _activeBoxes;

    /// <summary>
    /// Launches a new desktop box with the specified configuration
    /// </summary>
    /// <param name="box">The box configuration to launch</param>
    /// <returns>True if the box was launched successfully</returns>
    public static bool LaunchBox(Box box)
    {
        try
        {
            // Create a new desktop box window
            var desktopBox = new DesktopBox(box);

            // Show the window
            desktopBox.Show();

            // Add to active boxes collection
            _activeBoxes.Add(desktopBox);

            System.Diagnostics.Debug.WriteLine($"BoxLauncher: Successfully launched box '{box.Name}' at position ({box.X}, {box.Y})");
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"BoxLauncher: Failed to launch box '{box.Name}': {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Launches a new desktop box with default settings
    /// </summary>
    /// <param name="name">Name for the new box</param>
    /// <param name="x">X position on screen</param>
    /// <param name="y">Y position on screen</param>
    /// <param name="width">Width of the box</param>
    /// <param name="height">Height of the box</param>
    /// <returns>True if the box was launched successfully</returns>
    public static bool LaunchBox(string name, double x, double y, double width = 400, double height = 300)
    {
        var box = new Box
        {
            Id = Guid.NewGuid(),
            Name = name,
            Description = $"Desktop box created at {DateTime.Now}",
            Style = BoxStyle.Windows,
            X = x,
            Y = y,
            Width = width,
            Height = height,
            IsVisible = true,
            CreatedDate = DateTime.Now
        };

        return LaunchBox(box);
    }

    /// <summary>
    /// Closes a specific desktop box
    /// </summary>
    /// <param name="box">The box to close</param>
    public static void CloseBox(DesktopBox box)
    {
        try
        {
            if (box != null && _activeBoxes.Contains(box))
            {
                box.Close();
                _activeBoxes.Remove(box);
                System.Diagnostics.Debug.WriteLine($"BoxLauncher: Closed box '{box.Title}'");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"BoxLauncher: Error closing box: {ex.Message}");
        }
    }

    /// <summary>
    /// Closes all active desktop boxes
    /// </summary>
    public static void CloseAllBoxes()
    {
        try
        {
            var boxesToClose = _activeBoxes.ToList(); // Create a copy to avoid modification during iteration
            foreach (var box in boxesToClose)
            {
                CloseBox(box);
            }
            System.Diagnostics.Debug.WriteLine($"BoxLauncher: Closed all {_activeBoxes.Count} active boxes");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"BoxLauncher: Error closing all boxes: {ex.Message}");
        }
    }

    /// <summary>
    /// Gets the count of currently active boxes
    /// </summary>
    public static int GetActiveBoxCount()
    {
        return _activeBoxes.Count;
    }

    /// <summary>
    /// Finds a box by its ID
    /// </summary>
    /// <param name="id">The ID of the box to find</param>
    /// <returns>The desktop box if found, null otherwise</returns>
    public static DesktopBox FindBoxById(Guid id)
    {
        return _activeBoxes.FirstOrDefault(box => box.Id == id);
    }

    /// <summary>
    /// Launches the Avalonia application for desktop boxes
    /// </summary>
    /// <param name="args">Command line arguments</param>
    public static void LaunchApplication(string[] args)
    {
        try
        {
            var builder = Program.BuildAvaloniaApp();
            builder.StartWithClassicDesktopLifetime(args);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"BoxLauncher: Failed to launch application: {ex.Message}");

            // Try to launch as a separate process if this fails
            LaunchAsSeparateProcess(args);
        }
    }

    /// <summary>
    /// Launches the application as a separate process
    /// </summary>
    /// <param name="args">Command line arguments to pass to the new process</param>
    private static void LaunchAsSeparateProcess(string[] args)
    {
        try
        {
            var exePath = Process.GetCurrentProcess().MainModule?.FileName;
            if (string.IsNullOrEmpty(exePath))
            {
                System.Diagnostics.Debug.WriteLine("BoxLauncher: Could not determine executable path");
                return;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = exePath,
                Arguments = string.Join(" ", args),
                UseShellExecute = false,
                CreateNoWindow = false,
                WorkingDirectory = Path.GetDirectoryName(exePath)
            };

            Process.Start(startInfo);
            System.Diagnostics.Debug.WriteLine("BoxLauncher: Launched application as separate process");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"BoxLauncher: Failed to launch as separate process: {ex.Message}");
        }
    }

    /// <summary>
    /// Sends a message to all active boxes to update their appearance
    /// </summary>
    /// <param name="style">The new style to apply</param>
    public static void UpdateAllBoxStyles(BoxStyle style)
    {
        foreach (var box in _activeBoxes)
        {
            try
            {
                box.ApplyBoxStyle(style);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BoxLauncher: Error updating style for box '{box.Title}': {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Arranges all boxes in a grid layout
    /// </summary>
    /// <param name="screenWidth">Width of the screen</param>
    /// <param name="screenHeight">Height of the screen</param>
    public static void ArrangeBoxesInGrid(int screenWidth = 1920, int screenHeight = 1080)
    {
        const int boxWidth = 400;
        const int boxHeight = 300;
        const int spacing = 20;

        var boxes = _activeBoxes.ToList();
        var boxesPerRow = Math.Max(1, (screenWidth - spacing) / (boxWidth + spacing));

        for (int i = 0; i < boxes.Count; i++)
        {
            var row = i / boxesPerRow;
            var col = i % boxesPerRow;

            var x = spacing + col * (boxWidth + spacing);
            var y = spacing + row * (boxHeight + spacing);

            try
            {
                boxes[i].Position = new PixelPoint((int)x, (int)y);
                boxes[i].Width = boxWidth;
                boxes[i].Height = boxHeight;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"BoxLauncher: Error positioning box {i}: {ex.Message}");
            }
        }
    }
}
