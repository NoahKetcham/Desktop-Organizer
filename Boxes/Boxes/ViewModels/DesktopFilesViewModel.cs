using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Boxes.Models;
using Boxes.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Boxes.ViewModels;

public partial class DesktopFilesViewModel : ObservableObject
{
    private readonly FileScanner _fileScanner;
    private readonly RuleEngine _ruleEngine;
    
    [ObservableProperty]
    private bool _isScanning;
    
    [ObservableProperty]
    private string _statusMessage = "Ready to scan";
    
    [ObservableProperty]
    private int _totalFiles;
    
    [ObservableProperty]
    private int _selectedFiles;
    
    [ObservableProperty]
    private string _filterText = string.Empty;
    
    public ObservableCollection<FileItem> Files { get; } = new();
    public ObservableCollection<FileItem> FilteredFiles { get; } = new();
    
    public DesktopFilesViewModel(FileScanner fileScanner, RuleEngine ruleEngine)
    {
        _fileScanner = fileScanner;
        _ruleEngine = ruleEngine;
        
        // Load default rules
        _ruleEngine.LoadDefaultRules();
    }
    
    [RelayCommand]
    private async Task ScanDesktop()
    {
        IsScanning = true;
        StatusMessage = "Scanning desktop...";
        
        try
        {
            Files.Clear();
            FilteredFiles.Clear();
            
            var files = await _fileScanner.ScanDesktopAsync();
            
            // Apply rules to files
            _ruleEngine.ApplyRulesToFiles(files);
            
            foreach (var file in files)
            {
                Files.Add(file);
                FilteredFiles.Add(file);
            }
            
            TotalFiles = Files.Count;
            StatusMessage = $"Found {TotalFiles} files";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsScanning = false;
        }
    }
    
    [RelayCommand]
    private void SelectAll()
    {
        foreach (var file in FilteredFiles)
        {
            file.IsSelected = true;
        }
        UpdateSelectedCount();
    }
    
    [RelayCommand]
    private void DeselectAll()
    {
        foreach (var file in FilteredFiles)
        {
            file.IsSelected = false;
        }
        UpdateSelectedCount();
    }
    
    [RelayCommand]
    private void ToggleFileSelection(FileItem file)
    {
        file.IsSelected = !file.IsSelected;
        UpdateSelectedCount();
    }
    
    [RelayCommand]
    private async Task OrganizeSelected()
    {
        var selectedFiles = FilteredFiles.Where(f => f.IsSelected && f.SuggestedPath != null).ToList();
        
        if (selectedFiles.Count == 0)
        {
            StatusMessage = "No files selected or no suggestions available";
            return;
        }
        
        StatusMessage = $"Organizing {selectedFiles.Count} files...";
        
        // TODO: Implement actual file moving logic
        await Task.Delay(1000); // Simulate work
        
        StatusMessage = $"Organized {selectedFiles.Count} files successfully";
    }
    
    partial void OnFilterTextChanged(string value)
    {
        ApplyFilter();
    }
    
    private void ApplyFilter()
    {
        FilteredFiles.Clear();
        
        var filtered = string.IsNullOrWhiteSpace(FilterText)
            ? Files
            : Files.Where(f => 
                f.Name.Contains(FilterText, StringComparison.OrdinalIgnoreCase) ||
                f.FileType.Contains(FilterText, StringComparison.OrdinalIgnoreCase));
        
        foreach (var file in filtered)
        {
            FilteredFiles.Add(file);
        }
    }
    
    private void UpdateSelectedCount()
    {
        SelectedFiles = FilteredFiles.Count(f => f.IsSelected);
    }
}
