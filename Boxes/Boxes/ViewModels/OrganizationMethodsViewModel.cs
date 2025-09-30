using System.Collections.ObjectModel;
using Boxes.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Boxes.ViewModels;

public partial class OrganizationMethodsViewModel : ObservableObject
{
    public ObservableCollection<OrganizationMethod> OrganizationMethods { get; } = new();

    public OrganizationMethodsViewModel()
    {
        LoadOrganizationMethods();
    }

    private void LoadOrganizationMethods()
    {
        // By File Type
        OrganizationMethods.Add(new OrganizationMethod
        {
            Name = "By File Type",
            Description = "Automatically sort files into folders based on their type (Documents, Images, Videos, etc.)",
            IconGlyph = "\uE8F7", // Folder library
            AccentColor = "#0078D4",
            Type = OrganizationMethodType.ByFileType,
            FilesAffected = 45,
            SupportedFileTypes = new[] { ".pdf", ".docx", ".xlsx", ".jpg", ".png", ".mp4" },
            IsEnabled = true
        });

        // By Date
        OrganizationMethods.Add(new OrganizationMethod
        {
            Name = "By Date Created",
            Description = "Organize files into year/month folders based on when they were created",
            IconGlyph = "\uE787", // Calendar
            AccentColor = "#8B5CF6",
            Type = OrganizationMethodType.ByDate,
            FilesAffected = 38,
            SupportedFileTypes = new[] { "All files" },
            IsEnabled = false
        });

        // By Size
        OrganizationMethods.Add(new OrganizationMethod
        {
            Name = "By File Size",
            Description = "Separate large files from small ones to optimize storage and access",
            IconGlyph = "\uE9F9", // Storage
            AccentColor = "#10B981",
            Type = OrganizationMethodType.BySize,
            FilesAffected = 52,
            SupportedFileTypes = new[] { "All files" },
            IsEnabled = false
        });

        // By Name Pattern
        OrganizationMethods.Add(new OrganizationMethod
        {
            Name = "By Name Pattern",
            Description = "Group files with similar names or prefixes together (e.g., project names, dates)",
            IconGlyph = "\uE8AC", // Text
            AccentColor = "#F59E0B",
            Type = OrganizationMethodType.ByName,
            FilesAffected = 23,
            SupportedFileTypes = new[] { "All files" },
            IsEnabled = true
        });

        // By Project
        OrganizationMethods.Add(new OrganizationMethod
        {
            Name = "By Project",
            Description = "Smart detection of related files to group them into project folders",
            IconGlyph = "\uE8F1", // Briefcase
            AccentColor = "#EC4899",
            Type = OrganizationMethodType.ByProject,
            FilesAffected = 31,
            SupportedFileTypes = new[] { "Smart detection" },
            IsEnabled = false
        });

        // Screenshots
        OrganizationMethods.Add(new OrganizationMethod
        {
            Name = "Screenshot Organizer",
            Description = "Automatically move screenshots to a dedicated folder and organize by date",
            IconGlyph = "\uE7B4", // Camera
            AccentColor = "#06B6D4",
            Type = OrganizationMethodType.Custom,
            FilesAffected = 12,
            SupportedFileTypes = new[] { ".png (screenshots)" },
            IsEnabled = true
        });
    }

    [RelayCommand]
    private void ApplyAll()
    {
        // TODO: Apply all enabled organization methods
    }

    [RelayCommand]
    private void Configure()
    {
        // TODO: Open configuration dialog
    }
}
