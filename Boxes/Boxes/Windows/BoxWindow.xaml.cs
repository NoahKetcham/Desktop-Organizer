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
using Point = Windows.Foundation.Point;
using Size = Windows.Foundation.Size;

namespace Boxes.Windows;

/// <summary>
/// A floating desktop window that represents a Box container
/// </summary>
public sealed partial class BoxWindow : Window, INotifyPropertyChanged
{
    private readonly AppWindow _appWindow;
    private Box _boxData;

    // Drag and resize state (simplified)
    private bool _isDragging = false;
    private bool _isResizing = false;

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
        try
        {
            Console.WriteLine("BoxWindow constructor starting...");
            
            InitializeComponent();
            Console.WriteLine("InitializeComponent completed");
            
            _boxData = box;
            Console.WriteLine("BoxData set");
            
            // Get the AppWindow
            var hWnd = WindowNative.GetWindowHandle(this);
            Console.WriteLine("Got window handle");
            var windowId = Win32Interop.GetWindowIdFromWindow(hWnd);
            Console.WriteLine("Got window ID");
            _appWindow = AppWindow.GetFromWindowId(windowId);
            Console.WriteLine("Got AppWindow");
            
            // Configure window
            Title = box.Name;
            Console.WriteLine("Set title");
            ExtendsContentIntoTitleBar = true;
            Console.WriteLine("Set ExtendsContentIntoTitleBar");
            SetTitleBar(DragArea);
            Console.WriteLine("Set title bar");
            
            // Set initial position and size
            _appWindow.MoveAndResize(new RectInt32(
                (int)box.X,
                (int)box.Y,
                (int)box.Width,
                (int)box.Height
            ));
            Console.WriteLine("Set position and size");
            
            // Make it a tool window (doesn't show in taskbar)
            var presenter = _appWindow.Presenter as OverlappedPresenter;
            if (presenter != null)
            {
                presenter.IsResizable = !box.IsLocked;
                presenter.IsMaximizable = false;
                presenter.IsMinimizable = false;
            }
            Console.WriteLine("Configured presenter");
            
            // Apply visual style
            ApplyStyle(box.Style, box.Opacity);
            Console.WriteLine("Applied style");
            
            UpdateWindowAppearance();
            Console.WriteLine("Updated window appearance");

            // Set up drag and resize event handlers
            SetupInteractionHandlers();
            Console.WriteLine("Setup interaction handlers completed");
            
            Console.WriteLine("BoxWindow constructor completed successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"BoxWindow constructor error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    private void SetupInteractionHandlers()
    {
        // Drag functionality
        DragArea.PointerPressed += DragArea_PointerPressed;
        DragArea.PointerMoved += DragArea_PointerMoved;
        DragArea.PointerReleased += DragArea_PointerReleased;

        // Resize functionality for resize handle
        ResizeHandle.PointerPressed += ResizeHandle_PointerPressed;
        ResizeHandle.PointerReleased += ResizeHandle_PointerReleased;

        // Global pointer events for drag continuation
        this.Activated += BoxWindow_Activated;
    }

    private void DragArea_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (BoxData.IsLocked) return;

        _isDragging = true;

        // For now, we'll implement a simpler drag mechanism
        // Visual feedback - slightly dim the window during drag
        MainContainer.Opacity *= 0.8;

        e.Handled = true;
    }

    private void DragArea_PointerMoved(object sender, PointerRoutedEventArgs e)
    {
        // Simplified drag implementation - for now just provide visual feedback
        if (!_isDragging || BoxData.IsLocked) return;

        e.Handled = true;
    }

    private void DragArea_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (!_isDragging) return;

        _isDragging = false;

        // Restore opacity
        MainContainer.Opacity = BoxData.Opacity;

        // Update modification date
        BoxData.ModifiedDate = DateTime.Now;

        e.Handled = true;
    }

    private void ResizeHandle_PointerPressed(object sender, PointerRoutedEventArgs e)
    {
        if (BoxData.IsLocked) return;

        _isResizing = true;

        // Visual feedback - show resize cursor
        _appWindow.Title = $"{BoxData.Name} - Resizing";

        e.Handled = true;
    }


    private void ResizeHandle_PointerReleased(object sender, PointerRoutedEventArgs e)
    {
        if (!_isResizing) return;

        _isResizing = false;

        // Restore title
        _appWindow.Title = BoxData.Name;

        // Update modification date
        BoxData.ModifiedDate = DateTime.Now;

        e.Handled = true;
    }

    private void BoxWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        // End any ongoing drag operations if window loses focus
        if (args.WindowActivationState == WindowActivationState.Deactivated)
        {
            if (_isDragging)
            {
                _isDragging = false;
                MainContainer.Opacity = BoxData.Opacity;
            }
            if (_isResizing)
            {
                _isResizing = false;
                _appWindow.Title = BoxData.Name;
            }
        }
    }



    private void UpdateWindowAppearance()
    {
        if (BoxData == null) return;

        // Update title and visual elements
        Title = BoxData.Name;
        BoxTitleText.Text = $"{BoxData.IconGlyph} {BoxData.Name}";
    }

    private void ApplyStyle(BoxStyle style, double opacity)
    {
        // Apply opacity to main container
        MainContainer.Opacity = opacity;

        // Reset visual elements
        SystemBackdrop = null;

        // Hide enhanced glass border by default
        OuterGlassBorder.Visibility = Visibility.Collapsed;

        // Parse box color for accenting
        var boxColor = ParseColor(BoxData.Color);

        // Apply backdrop and styling based on style
        switch (style)
        {
            case BoxStyle.Windows:
                // Enhanced Mica backdrop with subtle color tint
                SystemBackdrop = new MicaBackdrop();
                ApplyWindowsStyle(boxColor);
                break;

            case BoxStyle.Acetate:
                // Ultra-transparent acetate with enhanced glass effects
                ApplyAcetateStyle(boxColor);
                break;

            case BoxStyle.Solid:
                // Solid color with enhanced styling
                ApplySolidStyle(boxColor);
                break;

            case BoxStyle.Acrylic:
                // Desktop acrylic with enhanced blur
                SystemBackdrop = new DesktopAcrylicBackdrop();
                ApplyAcrylicStyle(boxColor);
                break;

            case BoxStyle.Minimal:
                // Minimal enhanced styling
                ApplyMinimalStyle(boxColor);
                break;

            case BoxStyle.Frosted:
                // Frosted glass with enhanced opacity
                SystemBackdrop = new DesktopAcrylicBackdrop();
                ApplyFrostedStyle(boxColor);
                break;
        }

        // Update glass gradient with box color influence
        UpdateGlassGradient(boxColor);
    }

    private void ApplyWindowsStyle(WinUIColor accentColor)
    {
        // Enhanced Mica with subtle background
        RootGrid.Background = new SolidColorBrush(WinUIColor.FromArgb(25, 255, 255, 255));

        // Subtle border with accent hint
        var borderBrush = new LinearGradientBrush();
        var gradientStop1 = new GradientStop { Color = WinUIColor.FromArgb(120, accentColor.R, accentColor.G, accentColor.B), Offset = 0 };
        var gradientStop2 = new GradientStop { Color = WinUIColor.FromArgb(80, 0, 0, 0), Offset = 1 };
        borderBrush.GradientStops.Add(gradientStop1);
        borderBrush.GradientStops.Add(gradientStop2);
        RootGrid.BorderBrush = borderBrush;
        RootGrid.BorderThickness = new Thickness(1);
    }

    private void ApplyAcetateStyle(WinUIColor accentColor)
    {
        // Show enhanced glass border
        OuterGlassBorder.Visibility = Visibility.Visible;

        // Ultra-transparent with enhanced glass gradient
        RootGrid.Background = new SolidColorBrush(WinUIColor.FromArgb(0, 0, 0, 0));

        // Subtle reflective border
        var borderBrush = new LinearGradientBrush();
        var gs1 = new GradientStop { Color = WinUIColor.FromArgb(20, 255, 255, 255), Offset = 0 };
        var gs2 = new GradientStop { Color = WinUIColor.FromArgb(5, accentColor.R, accentColor.G, accentColor.B), Offset = 0.5 };
        var gs3 = new GradientStop { Color = WinUIColor.FromArgb(20, 255, 255, 255), Offset = 1 };
        borderBrush.GradientStops.Add(gs1);
        borderBrush.GradientStops.Add(gs2);
        borderBrush.GradientStops.Add(gs3);

        RootGrid.BorderBrush = borderBrush;
        RootGrid.BorderThickness = new Thickness(1);
    }

    private void ApplySolidStyle(WinUIColor accentColor)
    {
        // Solid background with accent color
        RootGrid.Background = new SolidColorBrush(WinUIColor.FromArgb(255, accentColor.R, accentColor.G, accentColor.B));

        // Matching border
        RootGrid.BorderBrush = new SolidColorBrush(WinUIColor.FromArgb(255, (byte)(accentColor.R * 0.8), (byte)(accentColor.G * 0.8), (byte)(accentColor.B * 0.8)));
        RootGrid.BorderThickness = new Thickness(2);
    }

    private void ApplyAcrylicStyle(WinUIColor accentColor)
    {
        // Acrylic background with color tint
        var bgColor = WinUIColor.FromArgb(50,
            (byte)((accentColor.R + 255) / 2),
            (byte)((accentColor.G + 255) / 2),
            (byte)((accentColor.B + 255) / 2));
        RootGrid.Background = new SolidColorBrush(bgColor);

        // Enhanced border
        RootGrid.BorderBrush = new SolidColorBrush(WinUIColor.FromArgb(150, 255, 255, 255));
        RootGrid.BorderThickness = new Thickness(1);
    }

    private void ApplyMinimalStyle(WinUIColor accentColor)
    {
        // Minimal transparent background
        RootGrid.Background = new SolidColorBrush(WinUIColor.FromArgb(8, 255, 255, 255));

        // Very subtle accent border
        RootGrid.BorderBrush = new SolidColorBrush(WinUIColor.FromArgb(60, accentColor.R, accentColor.G, accentColor.B));
        RootGrid.BorderThickness = new Thickness(1);
    }

    private void ApplyFrostedStyle(WinUIColor accentColor)
    {
        // Frosted background with color influence
        var bgColor = WinUIColor.FromArgb(160,
            (byte)((accentColor.R + 245) / 2),
            (byte)((accentColor.G + 245) / 2),
            (byte)((accentColor.B + 245) / 2));
        RootGrid.Background = new SolidColorBrush(bgColor);

        // Enhanced frosted border
        RootGrid.BorderBrush = new SolidColorBrush(WinUIColor.FromArgb(220, 255, 255, 255));
        RootGrid.BorderThickness = new Thickness(2);
    }

    private void UpdateGlassGradient(WinUIColor accentColor)
    {
        if (GlassGradient == null) return;

        // Update the glass gradient to subtly reflect the box color
        var centerColor = WinUIColor.FromArgb(50,
            (byte)((accentColor.R + 255) / 2),
            (byte)((accentColor.G + 255) / 2),
            (byte)((accentColor.B + 255) / 2));

        // The gradient is defined in XAML, but we could enhance it here if needed
    }

    private WinUIColor ParseColor(string colorHex)
    {
        if (string.IsNullOrEmpty(colorHex) || !colorHex.StartsWith("#"))
            return WinUIColor.FromArgb(255, 0, 120, 212); // Default blue

        try
        {
            var hex = colorHex.Substring(1);
            if (hex.Length == 6)
            {
                var r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                var g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                var b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                return WinUIColor.FromArgb(255, r, g, b);
            }
        }
        catch { }

        return WinUIColor.FromArgb(255, 0, 120, 212); // Default blue
    }
    

    public event PropertyChangedEventHandler? PropertyChanged;

    private void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

