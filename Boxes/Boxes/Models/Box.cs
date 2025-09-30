using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Boxes.Models;

/// <summary>
/// Represents a desktop container box that can hold files
/// </summary>
public class Box
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = "New Box";
    public string Description { get; set; } = string.Empty;
    public string Color { get; set; } = "#0078D4"; // Default blue
    public string IconGlyph { get; set; } = "\uE7B8"; // Box icon
    
    // Position on desktop
    public double X { get; set; } = 100;
    public double Y { get; set; } = 100;
    
    // Size
    public double Width { get; set; } = 300;
    public double Height { get; set; } = 400;
    
    // Behavior
    public bool IsVisible { get; set; } = true;
    public bool IsLocked { get; set; } = false; // Prevent moving/resizing
    public bool AlwaysOnTop { get; set; } = false;
    
    // Files in this box
    public ObservableCollection<FileItem> Files { get; set; } = new();
    
    // Organization rules for this box
    public ObservableCollection<OrganizationRule> Rules { get; set; } = new();
    
    // Metadata
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime ModifiedDate { get; set; } = DateTime.Now;
    
    // Stats
    public int FileCount => Files.Count;
    public long TotalSize => Files.Sum(f => f.SizeInBytes);
    public string FormattedTotalSize => FormatFileSize(TotalSize);
    
    private static string FormatFileSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double len = bytes;
        int order = 0;
        while (len >= 1024 && order < sizes.Length - 1)
        {
            order++;
            len = len / 1024;
        }
        return $"{len:0.##} {sizes[order]}";
    }
}

