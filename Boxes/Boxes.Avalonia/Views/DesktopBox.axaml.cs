using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Boxes.Avalonia.Services;
using Boxes.Models;
using System;

namespace Boxes.Avalonia.Views;

public partial class DesktopBox : Window
{
    private readonly Box _boxData;
    private bool _isDragging;
    private Point _dragStartPoint;
    private Point _windowStartPosition;

    // Start crystal-clear (no blur). Keep for future toggle if you want acrylic later.
    private bool _seeThroughEnabled = true;

    public Guid Id => _boxData.Id;

    public DesktopBox()
    {
        InitializeComponent();

        // Default box for testing
        _boxData = new Box
        {
            Id = Guid.NewGuid(),
            Name = "Desktop Box",
            Description = "Liquid-glass desktop container",
            Style = BoxStyle.Acetate, // start on acetate
            X = 100,
            Y = 100,
            Width = 400,
            Height = 300,
            IsVisible = true,
            CreatedDate = DateTime.Now
        };

        SetupWindow();
    }

    public DesktopBox(Box box)
    {
        InitializeComponent();
        _boxData = box ?? throw new ArgumentNullException(nameof(box));
        SetupWindow();
    }

    private void SetupWindow()
    {
        // Window surface
        Title = _boxData.Name;
        Width = _boxData.Width;
        Height = _boxData.Height;
        Position = new PixelPoint((int)_boxData.X, (int)_boxData.Y);
        WindowStartupLocation = WindowStartupLocation.Manual;
        ShowInTaskbar = false;
        Topmost = true;
        CanResize = true;
        SystemDecorations = SystemDecorations.None;

        TransparencyLevelHint = new[] { WindowTransparencyLevel.Transparent };
        Background = Brushes.Transparent;

        UpdateAcetateEffect();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        // Drag/drop events (kept)
        this.AddHandler(DragDrop.DropEvent, DropZone_Drop);
        this.AddHandler(DragDrop.DragOverEvent, DropZone_DragOver);

        // Window dragging
        this.PointerPressed  += DragArea_PointerPressed;
        this.PointerMoved    += DragArea_PointerMoved;
        this.PointerReleased += DragArea_PointerReleased;
    }

    #region Drag Functionality

    private void DragArea_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            _isDragging = true;
            _dragStartPoint = e.GetPosition(this);
            _windowStartPosition = new Point(this.Position.X, this.Position.Y);
            e.Pointer.Capture(this);
        }
    }

    private void DragArea_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isDragging && e.Pointer.Captured == this)
        {
            var current = e.GetPosition(this);
            var deltaX = current.X - _dragStartPoint.X;
            var deltaY = current.Y - _dragStartPoint.Y;

            var newX = _windowStartPosition.X + deltaX;
            var newY = _windowStartPosition.Y + deltaY;

            var screen = Screens.Primary;
            if (screen != null)
            {
                newX = Math.Max(0, Math.Min(newX, screen.Bounds.Width - this.Width));
                newY = Math.Max(0, Math.Min(newY, screen.Bounds.Height - this.Height));
            }

            this.Position = new PixelPoint((int)newX, (int)newY);
        }
    }

    private void DragArea_PointerReleased(object? sender, PointerEventArgs e)
    {
        if (_isDragging)
        {
            _isDragging = false;
            e.Pointer.Capture(null);
        }
    }

    #endregion

    #region Event Handlers

    private void CloseButton_Click(object? sender, RoutedEventArgs e) => Close();

    private void DropZone_DragOver(object? sender, DragEventArgs e)
    {
        if (e.Data.GetDataFormats().Contains("File"))
            e.DragEffects = DragDropEffects.Copy;
        else
            e.DragEffects = DragDropEffects.None;
    }

    private void DropZone_Drop(object? sender, DragEventArgs e)
    {
        if (e.Data.GetDataFormats().Contains("File"))
        {
            var files = e.Data.GetFiles();
            if (files != null)
            {
                foreach (var f in files)
                    Console.WriteLine($"File dropped: {f.Name}");
            }
        }
    }

    #endregion

    #region Style / Rendering

    public void SetSeeThroughEnabled(bool enabled)
    {
        _seeThroughEnabled = enabled;
        UpdateAcetateEffect();
    }

    public void ApplyBoxStyle(BoxStyle style)
    {
        _boxData.Style = style;
        UpdateAcetateEffect();
    }

    private void UpdateAcetateEffect()
    {
        try
        {
            var outer  = this.FindControl<Border>("AcetateBorder");
            var inner  = this.FindControl<Border>("InnerRing");
            var center = this.FindControl<Grid>("ContentGrid");
            if (outer is null || inner is null || center is null) return;

            // Center must remain crystal clear.
            center.Background = Brushes.Transparent;

            // Optional: acrylic blur later by flipping _seeThroughEnabled.
            if (_seeThroughEnabled)
            {
                TransparencyLevelHint = new[] { WindowTransparencyLevel.Transparent };
                outer.Background = Brushes.Transparent;
            }
            else
            {
                TransparencyLevelHint = new[]
                {
                    WindowTransparencyLevel.AcrylicBlur,
                    WindowTransparencyLevel.Transparent
                };
                outer.Background = new SolidColorBrush(Color.FromArgb(16, 255, 255, 255));
            }

            // Two rings with stronger, complementary highlights so they read on busy wallpapers.
            switch (_boxData.Style)
            {
                case BoxStyle.Acetate:
                default:
                    // OUTER rim: thicker and brighter along TL→BR
                    outer.BorderThickness = new Thickness(3);
                    outer.BorderBrush = new LinearGradientBrush
                    {
                        StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                        EndPoint   = new RelativePoint(1, 1, RelativeUnit.Relative),
                        GradientStops =
                        {
                            // bright corners with a cool tint like clear acetate
                            new GradientStop(Color.Parse("#C0F5FAFF"), 0.00),
                            new GradientStop(Color.Parse("#88FFFFFF"), 0.15),
                            new GradientStop(Color.Parse("#48FFFFFF"), 0.55),
                            new GradientStop(Color.Parse("#70F5FAFF"), 0.85),
                            new GradientStop(Color.Parse("#A0FFFFFF"), 1.00),
                        }
                    };

                    // INNER rim: opposite diagonal (BL→TR) for the refraction vibe
                    inner.BorderThickness = new Thickness(2);
                    inner.BorderBrush = new LinearGradientBrush
                    {
                        StartPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
                        EndPoint   = new RelativePoint(1, 0, RelativeUnit.Relative),
                        GradientStops =
                        {
                            new GradientStop(Color.Parse("#9CFFFFFF"), 0.00),
                            new GradientStop(Color.Parse("#60FFFFFF"), 0.40),
                            new GradientStop(Color.Parse("#38FFFFFF"), 0.75),
                            new GradientStop(Color.Parse("#5AF5FAFF"), 1.00),
                        }
                    };
                    break;

                case BoxStyle.Minimal:
                    outer.BorderThickness = new Thickness(2);
                    outer.BorderBrush = new SolidColorBrush(Color.FromArgb(120, 255, 255, 255));
                    inner.BorderThickness = new Thickness(1.5);
                    inner.BorderBrush = new SolidColorBrush(Color.FromArgb(64, 255, 255, 255));
                    break;

                case BoxStyle.Windows:
                    outer.BorderThickness = new Thickness(3);
                    outer.BorderBrush = new LinearGradientBrush
                    {
                        StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                        EndPoint   = new RelativePoint(1, 1, RelativeUnit.Relative),
                        GradientStops =
                        {
                            new GradientStop(Color.FromArgb(168,   0, 120, 212), 0.05),
                            new GradientStop(Color.FromArgb(128,   0, 140, 240), 0.45),
                            new GradientStop(Color.FromArgb( 96,   0, 160, 255), 1.00),
                        }
                    };
                    inner.BorderThickness = new Thickness(2);
                    inner.BorderBrush = new SolidColorBrush(Color.FromArgb(72, 255, 255, 255));
                    break;

                case BoxStyle.Frosted:
                case BoxStyle.Acrylic:
                    outer.BorderThickness = new Thickness(3);
                    outer.BorderBrush = new LinearGradientBrush
                    {
                        StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                        EndPoint   = new RelativePoint(1, 1, RelativeUnit.Relative),
                        GradientStops =
                        {
                            new GradientStop(Color.FromArgb(160, 255, 255, 255), 0.0),
                            new GradientStop(Color.FromArgb( 88, 235, 235, 235), 0.6),
                            new GradientStop(Color.FromArgb( 56, 220, 220, 220), 1.0),
                        }
                    };
                    inner.BorderThickness = new Thickness(2);
                    inner.BorderBrush = new SolidColorBrush(Color.FromArgb(80, 255, 255, 255));
                    break;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error updating acetate effect: {ex.Message}");
        }
    }

    #endregion
}
