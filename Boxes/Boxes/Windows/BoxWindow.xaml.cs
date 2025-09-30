using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using Boxes.Models;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Windows.Graphics;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;

namespace Boxes.Windows;

/// <summary>
/// A floating desktop window that represents a Box container
/// </summary>
public sealed partial class BoxWindow : Window, INotifyPropertyChanged
{
    private readonly AppWindow _appWindow;
    private Box _boxData;
    
    public Box BoxData
    {
        get => _boxData;
        set
        {
            _boxData = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(StatusText));
            OnPropertyChanged(nameof(IsEmpty));
            UpdateWindowAppearance();
        }
    }

    public string StatusText
    {
        get
        {
            if (BoxData == null) return "";
            return BoxData.FileCount == 0 
                ? "Empty" 
                : $"{BoxData.FileCount} file{(BoxData.FileCount != 1 ? "s" : "")}";
        }
    }

    public Visibility IsEmpty => BoxData?.FileCount == 0 
        ? Visibility.Visible 
        : Visibility.Collapsed;

    public BoxWindow(Box box)
    {
        InitializeComponent();
        
        _boxData = box;
        
        // Get the AppWindow
        var hWnd = WindowNative.GetWindowHandle(this);
        var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
        _appWindow = AppWindow.GetFromWindowId(windowId);
        
        // Configure window
        Title = box.Name;
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(DragArea);
        
        // Set initial position and size
        _appWindow.MoveAndResize(new RectInt32(
            (int)box.X,
            (int)box.Y,
            (int)box.Width,
            (int)box.Height
        ));
        
        // Make it a tool window (doesn't show in taskbar)
        var presenter = _appWindow.Presenter as OverlappedPresenter;
        if (presenter != null)
        {
            presenter.IsResizable = !box.IsLocked;
            presenter.IsMaximizable = false;
            presenter.IsMinimizable = false;
        }
        
        UpdateWindowAppearance();
        UpdateUI();
        
        // Listen to box changes
        BoxData.Files.CollectionChanged += (s, e) =>
        {
            OnPropertyChanged(nameof(StatusText));
            OnPropertyChanged(nameof(IsEmpty));
            UpdateUI();
        };
    }

    private void UpdateWindowAppearance()
    {
        if (BoxData == null) return;
        
        // Update title
        Title = BoxData.Name;
    }

    private void UpdateUI()
    {
        if (BoxData == null) return;
        
        DispatcherQueue.TryEnqueue(() =>
        {
            BoxNameText.Text = BoxData.Name;
            BoxIcon.Glyph = BoxData.IconGlyph;
            BoxStatusText.Text = StatusText;
            FooterStats.Text = $"{BoxData.FileCount} file{(BoxData.FileCount != 1 ? "s" : "")} · {BoxData.FormattedTotalSize}";
            EmptyState.Visibility = IsEmpty;
            FilesList.ItemsSource = BoxData.Files;
        });
    }

    private void DragArea_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (BoxData.IsLocked) return;
        
        // Enable window dragging
        var presenter = _appWindow.Presenter as OverlappedPresenter;
        if (presenter != null)
        {
            // Store position when done dragging
            var position = _appWindow.Position;
            BoxData.X = position.X;
            BoxData.Y = position.Y;
        }
    }

    private void SettingsButton_Click(object sender, RoutedEventArgs e)
    {
        // TODO: Show box settings dialog
        // For now, just show a simple message
        var dialog = new ContentDialog
        {
            Title = "Box Settings",
            Content = $"Settings for '{BoxData.Name}' coming soon!",
            CloseButtonText = "Close",
            XamlRoot = Content.XamlRoot
        };
        _ = dialog.ShowAsync();
    }

    private void CloseButton_Click(object sender, RoutedEventArgs e)
    {
        // Hide the box instead of closing
        BoxData.IsVisible = false;
        _appWindow.Hide();
    }

    private async void AddFilesButton_Click(object sender, RoutedEventArgs e)
    {
        // TODO: Implement file picker
        var picker = new FileOpenPicker
        {
            ViewMode = PickerViewMode.List,
            SuggestedStartLocation = PickerLocationId.Desktop
        };
        
        picker.FileTypeFilter.Add("*");
        
        var hWnd = WindowNative.GetWindowHandle(this);
        WinRT.Interop.InitializeWithWindow.Initialize(picker, hWnd);
        
        var files = await picker.PickMultipleFilesAsync();
        if (files != null && files.Count > 0)
        {
            foreach (var file in files)
            {
                var basicProps = await file.GetBasicPropertiesAsync();
                var fileItem = new FileItem
                {
                    FullPath = file.Path,
                    Name = file.Name,
                    Extension = file.FileType,
                    CreatedDate = file.DateCreated.DateTime,
                    ModifiedDate = basicProps.DateModified.DateTime,
                    SizeInBytes = (long)basicProps.Size
                };

                // Set icon based on file type
                fileItem.IconGlyph = GetIconForFileType(file.FileType);

                BoxData.Files.Add(fileItem);
            }

            BoxData.ModifiedDate = DateTime.Now;
        }
    }

    private string GetIconForFileType(string extension)
    {
        return extension.ToLower() switch
        {
            ".pdf" or ".doc" or ".docx" or ".txt" => "\uE8A5", // Document
            ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" or ".svg" => "\uEB9F", // Image
            ".mp4" or ".avi" or ".mkv" or ".mov" => "\uE714", // Video
            ".mp3" or ".wav" or ".flac" => "\uE8D6", // Music
            ".zip" or ".rar" or ".7z" => "\uE8B7", // Archive
            ".exe" or ".msi" => "\uE756", // Application
            _ => "\uE8A5" // Default file
        };
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

