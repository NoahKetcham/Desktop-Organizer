using System;
using System.Collections.ObjectModel;
using System.Linq;
using Boxes.Models;
// using Boxes.Windows; // Temporarily commented out

namespace Boxes.Services;

/// <summary>
/// Manages all Box instances and their desktop windows
/// </summary>
public class BoxManager
{
    // private readonly Dictionary<Guid, BoxWindow> _activeWindows = new(); // Temporarily commented out
    
    public ObservableCollection<Box> Boxes { get; } = new();

    public BoxManager()
    {
        // Create a sample box for demo purposes
        CreateSampleBox();
    }

    private void CreateSampleBox()
    {
        var sampleBox = new Box
        {
            Name = "My First Box",
            Description = "A sample box to get you started",
            Color = "#0078D4",
            X = 100,
            Y = 100,
            Width = 300,
            Height = 400
        };
        
        Boxes.Add(sampleBox);
    }

    /// <summary>
    /// Creates a new box with default settings
    /// </summary>
    public Box CreateBox(string name = "New Box")
    {
        var box = new Box
        {
            Name = name,
            Color = GetRandomColor(),
            X = 100 + (Boxes.Count * 30),
            Y = 100 + (Boxes.Count * 30)
        };
        
        Boxes.Add(box);
        return box;
    }

    /// <summary>
    /// Shows a box on the desktop
    /// </summary>
    public void ShowBox(Box box)
    {
        // TODO: Implement desktop window display
        // For now, just mark as visible
        box.IsVisible = true;
        
        /* 
        if (_activeWindows.ContainsKey(box.Id))
        {
            // Window already exists, just activate it
            var window = _activeWindows[box.Id];
            window.Activate();
        }
        else
        {
            // Create new window
            var window = new BoxWindow(box);
            _activeWindows[box.Id] = window;
            
            // Remove from dictionary when closed
            window.Closed += (s, e) =>
            {
                _activeWindows.Remove(box.Id);
            };
            
            window.Activate();
        }
        
        box.IsVisible = true;
        */
    }

    /// <summary>
    /// Hides a box from the desktop
    /// </summary>
    public void HideBox(Box box)
    {
        // TODO: Implement desktop window hiding
        box.IsVisible = false;
        
        /*
        if (_activeWindows.TryGetValue(box.Id, out var window))
        {
            window.Hide();
            box.IsVisible = false;
        }
        */
    }

    /// <summary>
    /// Removes a box permanently
    /// </summary>
    public void DeleteBox(Box box)
    {
        // TODO: Close window if open
        /*
        if (_activeWindows.TryGetValue(box.Id, out var window))
        {
            window.Close();
            _activeWindows.Remove(box.Id);
        }
        */
        
        Boxes.Remove(box);
    }

    /// <summary>
    /// Shows all boxes
    /// </summary>
    public void ShowAllBoxes()
    {
        foreach (var box in Boxes)
        {
            ShowBox(box);
        }
    }

    /// <summary>
    /// Hides all boxes
    /// </summary>
    public void HideAllBoxes()
    {
        foreach (var box in Boxes)
        {
            HideBox(box);
        }
    }

    private string GetRandomColor()
    {
        var colors = new[]
        {
            "#0078D4", // Blue
            "#8764B8", // Purple
            "#00CC6A", // Green
            "#E81123", // Red
            "#FF8C00", // Orange
            "#00B7C3", // Teal
            "#68217A", // Deep Purple
            "#107C10"  // Dark Green
        };
        
        return colors[Random.Shared.Next(colors.Length)];
    }

    /// <summary>
    /// Gets the window instance for a box if it exists
    /// </summary>
    // public BoxWindow? GetWindowForBox(Box box)
    // {
    //     return _activeWindows.TryGetValue(box.Id, out var window) ? window : null;
    // }

    /// <summary>
    /// Updates box position and size from its window
    /// </summary>
    public void UpdateBoxFromWindow(Box box)
    {
        // TODO: Update from window
        /*
        if (_activeWindows.TryGetValue(box.Id, out var window))
        {
            // Position and size are updated in the BoxWindow itself
            box.ModifiedDate = DateTime.Now;
        }
        */
    }
}

