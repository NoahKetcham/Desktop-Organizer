using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Boxes.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Boxes.ViewModels;

public partial class TemplatesViewModel : ObservableObject
{
    public ObservableCollection<OrganizationSetup> Templates { get; } = new();
    public ObservableCollection<OrganizationSetup> UserSetups { get; } = new();

    public TemplatesViewModel()
    {
        LoadTemplates();
        LoadUserSetups();
    }

    private void LoadTemplates()
    {
        // Add pre-designed templates
        Templates.Add(new OrganizationSetup
        {
            Name = "Work Essentials",
            Description = "Organize documents, spreadsheets, and presentations into dedicated folders",
            Type = SetupType.Template,
            IconGlyph = "\uE8F1", // Briefcase
            RuleCount = 5,
            CategoryCount = 3,
            PreviewColor = "#0078D4",
            CategoryPreviews = new() { "Documents", "Spreadsheets", "Presentations" },
            RulePreviews = new() 
            { 
                "Move .docx, .doc files to Documents folder",
                "Move .xlsx, .xls files to Spreadsheets folder",
                "Move .pptx, .ppt files to Presentations folder",
                "Move .pdf files to Documents folder",
                "Archive files older than 90 days"
            }
        });

        Templates.Add(new OrganizationSetup
        {
            Name = "Creative Studio",
            Description = "Sort images, videos, and design files by project and file type",
            Type = SetupType.Template,
            IconGlyph = "\uE771", // Palette
            RuleCount = 8,
            CategoryCount = 4,
            PreviewColor = "#8B5CF6",
            CategoryPreviews = new() { "Images", "Videos", "Design Files", "Projects" },
            RulePreviews = new() 
            { 
                "Move .jpg, .png, .gif files to Images folder",
                "Move .mp4, .mov, .avi files to Videos folder",
                "Move .psd, .ai, .sketch files to Design Files folder",
                "Move .fig files to Design Files folder",
                "Group files by date into Projects subfolders",
                "Move raw camera files to Images/RAW folder",
                "Archive projects older than 6 months",
                "Sort images by resolution (HD, 4K, etc.)"
            }
        });

        Templates.Add(new OrganizationSetup
        {
            Name = "Developer Workspace",
            Description = "Manage code files, documentation, and development tools efficiently",
            Type = SetupType.Template,
            IconGlyph = "\uE943", // Code
            RuleCount = 6,
            CategoryCount = 5,
            PreviewColor = "#10B981",
            CategoryPreviews = new() { "Code", "Documentation", "Tools", "Configs", "Archives" },
            RulePreviews = new() 
            { 
                "Move source code files (.cs, .js, .py, etc.) to Code folder",
                "Move .md, .txt documentation to Documentation folder",
                "Move .json, .yaml config files to Configs folder",
                "Move .zip, .tar.gz archives to Archives folder",
                "Group code files by language",
                "Keep README files in root"
            }
        });

        Templates.Add(new OrganizationSetup
        {
            Name = "Student Hub",
            Description = "Keep assignments, research papers, and study materials organized",
            Type = SetupType.Template,
            IconGlyph = "\uE7BE", // Education
            RuleCount = 4,
            CategoryCount = 3,
            PreviewColor = "#F59E0B",
            CategoryPreviews = new() { "Assignments", "Research", "Study Materials" },
            RulePreviews = new() 
            { 
                "Move assignment files to Assignments folder",
                "Move research papers and PDFs to Research folder",
                "Move study notes and textbooks to Study Materials folder",
                "Organize by semester or course"
            }
        });

        Templates.Add(new OrganizationSetup
        {
            Name = "Media Manager",
            Description = "Automatically organize music, photos, and videos by date and type",
            Type = SetupType.Template,
            IconGlyph = "\uE8B8", // Media
            RuleCount = 7,
            CategoryCount = 4,
            PreviewColor = "#EC4899",
            CategoryPreviews = new() { "Photos", "Videos", "Music", "Downloads" },
            RulePreviews = new() 
            { 
                "Move image files to Photos folder sorted by date",
                "Move video files to Videos folder sorted by date",
                "Move music files to Music folder sorted by artist",
                "Move downloads to Downloads folder",
                "Create year/month subfolders for media",
                "Move screenshots to Photos/Screenshots",
                "Archive media older than 1 year"
            }
        });

        Templates.Add(new OrganizationSetup
        {
            Name = "Minimalist",
            Description = "Simple two-folder setup: Current and Archive",
            Type = SetupType.Template,
            IconGlyph = "\uE8F4", // Checkbox
            RuleCount = 2,
            CategoryCount = 2,
            PreviewColor = "#6B7280",
            CategoryPreviews = new() { "Current", "Archive" },
            RulePreviews = new() 
            { 
                "Keep recent files (< 30 days) in Current folder",
                "Move older files to Archive folder"
            }
        });
    }

    private void LoadUserSetups()
    {
        // Load user's previously saved setups from storage
        // This would typically load from a database or JSON file
        // For now, we'll show a placeholder if user has no setups
    }

    [RelayCommand]
    private void CreateNewSetup()
    {
        // TODO: Open dialog to create new setup
    }

    [RelayCommand]
    private async Task SelectSetup(OrganizationSetup setup)
    {
        // Show template details dialog
        var dialog = new Dialogs.TemplateDetailsDialog(setup);
        
        // Set XamlRoot for the dialog (required for WinUI 3)
        dialog.XamlRoot = App.Current.Window?.Content?.XamlRoot;
        
        var result = await dialog.ShowAsync();
        
        if (result == Microsoft.UI.Xaml.Controls.ContentDialogResult.Primary)
        {
            // User clicked "Use This Template"
            // TODO: Apply the template (create folders, rules, etc.)
        }
    }
}
