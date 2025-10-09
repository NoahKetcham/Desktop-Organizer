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

            // OUTER rim: Softer, with a top-left to bottom-right subtle gradient.
            outer.BorderThickness = new Thickness(2);
            outer.BorderBrush = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.Parse("#A0FFFFFF"), 0.0),
                    new GradientStop(Color.Parse("#60FFFFFF"), 1.0),
                }
            };

            // INNER rim: Sharp highlights on top-right and bottom-left.
            inner.BorderThickness = new Thickness(1);
            inner.BorderBrush = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.Parse("#FFFFFFFF"), 0.0),
                    new GradientStop(Color.Parse("#20FFFFFF"), 0.5),
                    new GradientStop(Color.Parse("#FFFFFFFF"), 1.0),
                }
            };

            // Ring fill brush: A strong gradient to bridge the borders.
            ringFill.Stroke = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(1, 0, RelativeUnit.Relative),
                EndPoint = new RelativePoint(0, 1, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.Parse("#CCFFFFFF"), 0.0),
                    new GradientStop(Color.Parse("#11FFFFFF"), 0.5),
                    new GradientStop(Color.Parse("#BBFFFFFF"), 1.0),
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
        var outerT = outer.BorderThickness.Left;        // e.g., 2
        var innerT = inner.BorderThickness.Left;        // e.g., 1

        // To center the stroke, we define the ringFill rectangle's geometry.
        // The stroke is drawn centered on the rectangle's edge.
        var margin = outerT + innerT / 2.0;
        ringFill.Margin = new Thickness(margin);

        // The stroke thickness is the gap minus half of each border's thickness,
        // since the stroke extends outwards and inwards from its center.
        var stroke = Math.Max(0, gap - (outerT / 2.0) - (innerT / 2.0));
        ringFill.StrokeThickness = stroke;

        // The corner radius must be adjusted for the new margin.
        var outerRadius = outer.CornerRadius.TopLeft;
        var ringRadius = Math.Max(0, outerRadius - margin);
        ringFill.RadiusX = ringRadius;
        ringFill.RadiusY = ringRadius;
    }

    // If you upgrade Avalonia and enable Acrylic, re-add the donut geometry here.
    // (Left commented intentionally to respect your "don't remove" rule.)

    #endregion
}
