using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Controls.Shapes; // Rectangle
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

    private bool _seeThroughEnabled = true;

    public Guid Id => _boxData.Id;

    public DesktopBox()
    {
        InitializeComponent();

        _boxData = new Box
        {
            Id = Guid.NewGuid(),
            Name = "Desktop Box",
            Description = "Liquid-glass desktop container",
            Style = BoxStyle.Acetate,
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

        // Keep geometry snug if the window is resized
        this.GetObservable(BoundsProperty).Subscribe(_ => UpdateRingGeometryOnly());
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        // Drag/drop events (kept; Data is obsolete -> warnings OK on older Avalonia)
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
        // Older Avalonia: e.Data is marked obsolete; leaving for compatibility.
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
            var outer    = this.FindControl<Border>("AcetateBorder");
            var inner    = this.FindControl<Border>("InnerRing");
            var center   = this.FindControl<Grid>("ContentGrid");
            var ringFill = this.FindControl<Rectangle>("RingFill");
            if (outer is null || inner is null || center is null || ringFill is null) return;

            // Center remains crystal clear.
            center.Background = Brushes.Transparent;

            // OUTER + INNER rims (tuned for clear acetate look)
            outer.BorderThickness = new Thickness(3);
            outer.BorderBrush = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint   = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.Parse("#D0FFFFFF"), 0.00),
                    new GradientStop(Color.Parse("#7AFFFFFF"), 0.50),
                    new GradientStop(Color.Parse("#90F5FAFF"), 0.90),
                    new GradientStop(Color.Parse("#B0FFFFFF"), 1.00),
                }
            };

            inner.BorderThickness = new Thickness(2);
            inner.BorderBrush = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
                EndPoint   = new RelativePoint(1, 0, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.Parse("#A8FFFFFF"), 0.00),
                    new GradientStop(Color.Parse("#60FFFFFF"), 0.55),
                    new GradientStop(Color.Parse("#40FFFFFF"), 1.00),
                }
            };

            // Ring fill brush (subtle, no blur)
            ringFill.Stroke = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0.15, 0.0, RelativeUnit.Relative),
                EndPoint   = new RelativePoint(0.85, 1.0, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.Parse("#22F5FAFF"), 0.00),
                    new GradientStop(Color.Parse("#10FFFFFF"), 0.35),
                    new GradientStop(Color.Parse("#0CFFFFFF"), 0.65),
                    new GradientStop(Color.Parse("#18F5FAFF"), 1.00),
                }
            };

            // Place the ring precisely between the two lines.
            UpdateRingGeometryOnly();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error updating acetate effect: {ex.Message}");
        }
    }

    private void UpdateRingGeometryOnly()
    {
        var outer    = this.FindControl<Border>("AcetateBorder");
        var inner    = this.FindControl<Border>("InnerRing");
        var ringFill = this.FindControl<Rectangle>("RingFill");
        if (outer is null || inner is null || ringFill is null) return;

        var gap    = inner.Margin.Left;                 // e.g., 10
        var outerT = outer.BorderThickness.Left;        // e.g., 3
        var innerT = inner.BorderThickness.Left;        // e.g., 2

        // Stroke centered exactly between outer inner-edge and inner outer-edge:
        // S = gap + (outerT + innerT)/2
        var stroke = gap + (outerT + innerT) / 2.0;
        ringFill.StrokeThickness = stroke;

        // Corner radius of the ring path = outer radius minus S/2
        var outerRadius = outer.CornerRadius.TopLeft;
        var ringRadius = Math.Max(0, outerRadius - stroke / 2.0);
        ringFill.RadiusX = ringRadius;
        ringFill.RadiusY = ringRadius;
    }

    // If you upgrade Avalonia and enable Acrylic, re-add the donut geometry here.
    // (Left commented intentionally to respect your "don't remove" rule.)

    #endregion
}
