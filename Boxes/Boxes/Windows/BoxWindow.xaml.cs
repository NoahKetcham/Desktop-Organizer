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
using Microsoft.UI.Xaml.Media;
using Windows.Foundation;
using Windows.Graphics;
using Windows.Storage;
using Windows.Storage.Pickers;
using WinRT.Interop;
using WinUIColor = Windows.UI.Color;

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
            UpdateWindowAppearance();
        }
    }


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
        
        // Apply visual style
        ApplyStyle(box.Style, box.Opacity);
        
        UpdateWindowAppearance();
        // This minimal window has no dynamic content to update
    }

    private void UpdateWindowAppearance()
    {
        if (BoxData == null) return;
        
        // Update title
        Title = BoxData.Name;
    }

    private void ApplyStyle(BoxStyle style, double opacity)
    {
        // Apply opacity to root grid
        RootGrid.Opacity = opacity;

        // Reset base styling (no content elements in minimal shell)
        RootGrid.BorderThickness = new Thickness(1);
        RootGrid.BorderBrush = new SolidColorBrush(WinUIColor.FromArgb(30, 255, 255, 255));
        DragArea.Background = new SolidColorBrush(WinUIColor.FromArgb(0, 0, 0, 0));
        SystemBackdrop = null;
        
        // Hide glass border by default
        OuterGlassBorder.Visibility = Visibility.Collapsed;

        // Apply backdrop based on style
        switch (style)
        {
            case BoxStyle.Windows:
                // Mica backdrop (default Windows 11 look)
                SystemBackdrop = new MicaBackdrop();
                RootGrid.Background = new SolidColorBrush(WinUIColor.FromArgb(20, 255, 255, 255));
                RootGrid.BorderBrush = new SolidColorBrush(WinUIColor.FromArgb(100, 0, 0, 0));
                break;

            case BoxStyle.Acetate:
                // Ultra-transparent acetate - like clear glass with reflective edges
                // Use DesktopAcrylicBackdrop for true transparency with minimal blur
                SystemBackdrop = new DesktopAcrylicBackdrop();
                
                // Show reflective glass border layer
                OuterGlassBorder.Visibility = Visibility.Visible;
                
                // Transparent base — sheen is drawn in XAML with radial gradient
                RootGrid.Background = new SolidColorBrush(WinUIColor.FromArgb(0, 0, 0, 0));
                
                // Subtle internal stroke
                RootGrid.BorderBrush = new SolidColorBrush(WinUIColor.FromArgb(80, 255, 255, 255));
                RootGrid.BorderThickness = new Thickness(0);

                // Header drag area remains transparent
                DragArea.Background = new SolidColorBrush(WinUIColor.FromArgb(0, 0, 0, 0));
                break;

            case BoxStyle.Solid:
                // Solid color, no transparency
                RootGrid.Background = new SolidColorBrush(WinUIColor.FromArgb(255, 243, 243, 243));
                RootGrid.BorderBrush = new SolidColorBrush(WinUIColor.FromArgb(255, 200, 200, 200));
                break;

            case BoxStyle.Acrylic:
                // Desktop acrylic blur effect
                SystemBackdrop = new DesktopAcrylicBackdrop();
                RootGrid.Background = new SolidColorBrush(WinUIColor.FromArgb(35, 255, 255, 255));
                RootGrid.BorderBrush = new SolidColorBrush(WinUIColor.FromArgb(120, 255, 255, 255));
                break;

            case BoxStyle.Minimal:
                // Minimal thin border, mostly transparent
                RootGrid.Background = new SolidColorBrush(WinUIColor.FromArgb(5, 255, 255, 255));
                RootGrid.BorderBrush = new SolidColorBrush(WinUIColor.FromArgb(40, 255, 255, 255));
                break;

            case BoxStyle.Frosted:
                // Frosted glass effect with more opacity
                SystemBackdrop = new DesktopAcrylicBackdrop();
                RootGrid.Background = new SolidColorBrush(WinUIColor.FromArgb(140, 240, 240, 245));
                RootGrid.BorderBrush = new SolidColorBrush(WinUIColor.FromArgb(200, 255, 255, 255));
                break;
        }
    }
    

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

