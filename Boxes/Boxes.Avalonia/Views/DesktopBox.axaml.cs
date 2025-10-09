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

    // Use screen coordinates for drag, so window can be moved across displays and is more responsive.
    private PixelPoint? _dragWindowStartScreenPos;
    private Point? _dragPointerStartScreenPos;

    private void DragArea_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            // Get the pointer position in screen coordinates
            var pointerScreen = this.PointToScreen(e.GetPosition(this));
            if (pointerScreen is { } pointerScreenPos)
            {
                _isDragging = true;
                _dragPointerStartScreenPos = new Point(pointerScreenPos.X, pointerScreenPos.Y);
                _dragWindowStartScreenPos = this.Position;
                e.Pointer.Capture(this);
            }
        }
    }

    private void DragArea_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isDragging && e.Pointer.Captured == this && _dragPointerStartScreenPos.HasValue && _dragWindowStartScreenPos.HasValue)
        {
            // Get current pointer position in screen coordinates
            var pointerScreen = this.PointToScreen(e.GetPosition(this));
            if (pointerScreen is { } pointerScreenPos)
            {
                var deltaX = pointerScreenPos.X - _dragPointerStartScreenPos.Value.X;
                var deltaY = pointerScreenPos.Y - _dragPointerStartScreenPos.Value.Y;

                var newX = _dragWindowStartScreenPos.Value.X + (int)deltaX;
                var newY = _dragWindowStartScreenPos.Value.Y + (int)deltaY;

                // Clamp to virtual screen bounds (all displays)
                var allScreensBounds = Screens.All.Select(s => s.Bounds).Aggregate((a, b) =>
                    new PixelRect(
                        Math.Min(a.X, b.X),
                        Math.Min(a.Y, b.Y),
                        Math.Max(a.Right, b.Right) - Math.Min(a.X, b.X),
                        Math.Max(a.Bottom, b.Bottom) - Math.Min(a.Y, b.Y)
                    )
                );

                newX = Math.Max(allScreensBounds.X, Math.Min(newX, allScreensBounds.Right - (int)this.Width));
                newY = Math.Max(allScreensBounds.Y, Math.Min(newY, allScreensBounds.Bottom - (int)this.Height));

                this.Position = new PixelPoint(newX, newY);
            }
        }
    }

    private void DragArea_PointerReleased(object? sender, PointerEventArgs e)
    {
        if (_isDragging)
        {
            _isDragging = false;
            _dragPointerStartScreenPos = null;
            _dragWindowStartScreenPos = null;
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
            var border = this.FindControl<Border>("AcetateBorder");
            var content = this.FindControl<Grid>("ContentGrid");
            if (border is null || content is null) return;

            // The border's background is transparent to see through the window.
            border.Background = Brushes.Transparent;

            // The entire frame is now rendered using the BorderBrush on a thick border.
            border.BorderThickness = new Thickness(12);

            // This gradient creates the layered, glassy look for the frame.
            // To make the gradient smoother, add more stops and use a less harsh diagonal.
            border.BorderBrush = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0.05, 0.05, RelativeUnit.Relative),
                EndPoint = new RelativePoint(0.95, 0.95, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.Parse("#52FFFFFF"), 0.0),  // 0x90 -> 0x72
                    new GradientStop(Color.Parse("#33FFFFFF"), 0.12), // 0x40 -> 0x33
                    new GradientStop(Color.Parse("#19FFFFFF"), 0.25), // 0x20 -> 0x19
                    new GradientStop(Color.Parse("#0DFFFFFF"), 0.45), // 0x10 -> 0x0D
                    new GradientStop(Color.Parse("#06FFFFFF"), 0.65), // 0x08 -> 0x06
                    new GradientStop(Color.Parse("#19FFFFFF"), 0.80), // 0x20 -> 0x19
                    new GradientStop(Color.Parse("#40FFFFFF"), 1.0),  // 0xA0 -> 0x80
                }
            };

            // This recreates the original translucent background with a diagonal sheen.
            // Smoother gradient: more stops, less contrast, and a gentler angle.
            content.Background = new LinearGradientBrush
            {
                StartPoint = new RelativePoint(0.15, 0.10, RelativeUnit.Relative),
                EndPoint = new RelativePoint(0.85, 0.90, RelativeUnit.Relative),
                GradientStops =
                {
                    new GradientStop(Color.Parse("#12000000"), 0.0),
                    new GradientStop(Color.Parse("#18FFFFFF"), 0.18),
                    new GradientStop(Color.Parse("#10FFFFFF"), 0.35),
                    new GradientStop(Color.Parse("#08FFFFFF"), 0.55),
                    new GradientStop(Color.Parse("#10FFFFFF"), 0.75),
                    new GradientStop(Color.Parse("#12000000"), 1.0),
                }
            };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error updating acetate effect: {ex.Message}");
        }
    }

    // This method is no longer needed with the simplified single-border design.
    private void UpdateRingGeometryOnly() { }

    // If you upgrade Avalonia and enable Acrylic, re-add the donut geometry here.
    // (Left commented intentionally to respect your "don't remove" rule.)

    #endregion
}
