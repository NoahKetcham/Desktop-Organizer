using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Boxes.Models;
using Boxes.Windows;

namespace Boxes.Services;

/// <summary>
/// Manages all Box instances and their desktop windows
/// </summary>
public class BoxManager
{
    private readonly Dictionary<Guid, BoxWindow> _activeWindows = new();
    
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
            Style = BoxStyle.Acetate,
            Opacity = 1.0,
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
    public Box CreateBox(string name = "New Box", BoxStyle? style = null, double? opacity = null)
    {
        var box = new Box
        {
            Name = name,
            Color = GetRandomColor(),
            Style = style ?? GetRandomStyle(),
            Opacity = opacity ?? 0.95,
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
    }

    /// <summary>
    /// Hides a box from the desktop
    /// </summary>
    public void HideBox(Box box)
    {
        if (_activeWindows.TryGetValue(box.Id, out var window))
        {
            window.Close();
            _activeWindows.Remove(box.Id);
            box.IsVisible = false;
        }
    }

    /// <summary>
    /// Removes a box permanently
    /// </summary>
    public void DeleteBox(Box box)
    {
        // Close window if open
        if (_activeWindows.TryGetValue(box.Id, out var window))
        {
            window.Close();
            _activeWindows.Remove(box.Id);
        }
        
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

    private BoxStyle GetRandomStyle()
    {
        var styles = new[]
        {
            BoxStyle.Windows,
            BoxStyle.Acetate,
            BoxStyle.Acrylic,
            BoxStyle.Frosted
        };
        
        return styles[Random.Shared.Next(styles.Length)];
    }

    /// <summary>
    /// Gets the window instance for a box if it exists
    /// </summary>
    public BoxWindow? GetWindowForBox(Box box)
    {
        return _activeWindows.TryGetValue(box.Id, out var window) ? window : null;
    }

    /// <summary>
    /// Updates box position and size from its window
    /// </summary>
    public void UpdateBoxFromWindow(Box box)
    {
        if (_activeWindows.TryGetValue(box.Id, out var window))
        {
            // Position and size are updated in the BoxWindow itself
            box.ModifiedDate = DateTime.Now;
        }
    }

    /// <summary>
    /// Updates a box's visual style and refreshes its window if visible
    /// </summary>
    public void UpdateBoxStyle(Box box, BoxStyle style, double opacity)
    {
        box.Style = style;
        box.Opacity = opacity;
        box.ModifiedDate = DateTime.Now;
        
        // If window is open, close and reopen it to apply new style
        if (_activeWindows.TryGetValue(box.Id, out var window))
        {
            window.Close();
            _activeWindows.Remove(box.Id);
            ShowBox(box);
        }
    }
}


