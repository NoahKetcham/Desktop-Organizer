using System;

namespace Boxes.Models;

public class OrganizationMethod
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string IconGlyph { get; set; } = "\uE8B7"; // Default folder icon
    public string AccentColor { get; set; } = "#0078D4";
    public OrganizationMethodType Type { get; set; }
    public bool IsEnabled { get; set; } = true;
    public int FilesAffected { get; set; }
    public string[] SupportedFileTypes { get; set; } = Array.Empty<string>();
    
    // For preview/info
    public string ExampleBefore { get; set; } = string.Empty;
    public string ExampleAfter { get; set; } = string.Empty;
}

public enum OrganizationMethodType
{
    ByFileType,
    ByDate,
    BySize,
    ByName,
    ByProject,
    Custom
}
