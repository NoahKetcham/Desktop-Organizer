using System;

namespace Boxes.Models;

public class FileItem
{
    public string FullPath { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public long SizeInBytes { get; set; }
    public DateTime CreatedDate { get; set; }
    public DateTime ModifiedDate { get; set; }
    public string FileType { get; set; } = string.Empty; // Document, Image, Video, etc.
    public string IconGlyph { get; set; } = "\uE8A5"; // Default file icon
    
    // Organization info
    public string? SuggestedCategory { get; set; }
    public string? SuggestedPath { get; set; }
    public bool IsSelected { get; set; }
    public OrganizationRule? MatchedRule { get; set; }
    
    // Computed properties
    public string FormattedSize => FormatFileSize(SizeInBytes);
    public string RelativeDate => GetRelativeDate(ModifiedDate);
    
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
    
    private static string GetRelativeDate(DateTime date)
    {
        var span = DateTime.Now - date;
        if (span.TotalMinutes < 1) return "Just now";
        if (span.TotalMinutes < 60) return $"{(int)span.TotalMinutes}m ago";
        if (span.TotalHours < 24) return $"{(int)span.TotalHours}h ago";
        if (span.TotalDays < 7) return $"{(int)span.TotalDays}d ago";
        if (span.TotalDays < 30) return $"{(int)(span.TotalDays / 7)}w ago";
        if (span.TotalDays < 365) return $"{(int)(span.TotalDays / 30)}mo ago";
        return $"{(int)(span.TotalDays / 365)}y ago";
    }
}
