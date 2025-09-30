using System;
using System.Collections.Generic;

namespace Boxes.Models;

public class OrganizationSetup
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public SetupType Type { get; set; }
    public string IconGlyph { get; set; } = "\uE8B7"; // Default folder icon
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public DateTime LastModifiedDate { get; set; } = DateTime.Now;
    public int RuleCount { get; set; }
    public int CategoryCount { get; set; }
    public string PreviewColor { get; set; } = "#0078D4"; // Default accent color
    
    // For preview/details dialog
    public List<string> CategoryPreviews { get; set; } = new();
    public List<string> RulePreviews { get; set; } = new();
    
    // For future preview functionality
    public string? PreviewImagePath { get; set; }
}

public enum SetupType
{
    Template,
    UserSetup
}
