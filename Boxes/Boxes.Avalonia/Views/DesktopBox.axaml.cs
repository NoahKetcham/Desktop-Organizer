using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Controls.Shapes; // for Rectangle
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
            var outer    = this.FindControl<Border>("AcetateBorder");
            var inner    = this.FindControl<Border>("InnerRing");
            var center   = this.FindControl<Grid>("ContentGrid");
            var ringFill = this.FindControl<Rectangle>("RingFill");
            if (outer is null || inner is null || center is null || ringFill is null) return;

            // Center must remain crystal clear.
            center.Background = Brushes.Transparent;

            // Optional: acrylic blur later by flipping _seeThroughEnabled (kept for future).
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

            // --- Visual recipe for "clear acetate rim" ---

            // OUTER rim line (slightly brighter)
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

            // INNER rim line (complementary diagonal)
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

            // RING FILL (the gap): a stroke-only rounded rectangle.
            // StrokeThickness roughly matches the space between the two lines.
            // You can fine-tune below if you change InnerRing.Margin or border thicknesses.
            var gap = inner.Margin.Left;              // 10 by default
            var outerStroke = outer.BorderThickness.Left; // 3
            var innerStroke = inner.BorderThickness.Left; // 2

            // Stroke centered on outer edge -> use about 2*gap minus a bit for the two strokes.
            var stroke = Math.Max(1, (int)Math.Round(2 * gap - (outerStroke + innerStroke)));
            ringFill.StrokeThickness = stroke;

            // Soft anisotropic gradient to mimic refractive tint in acetate (subtle blue/cool)
            ringFill.Stroke = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0.15, 0.0, RelativeUnit.Relative),
                EndPoint   = new RelativePoint(0.85, 1.0, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.Parse("#22F5FAFF"), 0.00), // gentle cool highlight
                    new GradientStop(Color.Parse("#10FFFFFF"), 0.35), // milky core
                    new GradientStop(Color.Parse("#0CFFFFFF"), 0.65), // fade
                    new GradientStop(Color.Parse("#18F5FAFF"), 1.00),
                }
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error updating acetate effect: {ex.Message}");
        }
    }

    #endregion
}
