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

    // New fields for resizing
    private bool _isResizing;
    private enum ResizeDirection
    {
        None, Top, Bottom, Left, Right,
        TopLeft, TopRight, BottomLeft, BottomRight
    }
    private ResizeDirection _resizeDirection;
    private Size _windowStartSize;
    private const int ResizeSnapIncrement = 8;


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

        // Window dragging and resizing
        this.PointerPressed  += Window_PointerPressed;
        this.PointerMoved    += Window_PointerMoved;
        this.PointerReleased += Window_PointerReleased;
    }

    #region Drag and Resize Functionality

    private void Window_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            var point = e.GetPosition(this);
            _resizeDirection = GetResizeDirection(point);

            if (_resizeDirection != ResizeDirection.None)
            {
                _isResizing = true;
                _windowStartSize = this.ClientSize;
                _windowStartPosition = new Point(this.Position.X, this.Position.Y);
                _dragStartPoint = point;
                e.Pointer.Capture(this);

                var sizeDisplay = this.FindControl<Border>("SizeDisplay");
                if (sizeDisplay != null) sizeDisplay.IsVisible = true;
            }
            else
            {
                var border = this.FindControl<Border>("AcetateBorder");
                if (border is null) return;
                
                var borderThickness = border.BorderThickness.Top;
                var dragRect = new Rect(borderThickness, borderThickness, this.Bounds.Width - (borderThickness * 2), 32);

                if (dragRect.Contains(point))
                {
                    _isDragging = true;
                    _dragStartPoint = point;
                    _windowStartPosition = new Point(this.Position.X, this.Position.Y);
                    e.Pointer.Capture(this);
                }
            }
        }
    }

    private void Window_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isResizing)
        {
            var currentPoint = e.GetPosition(this);
            var delta = currentPoint - _dragStartPoint;

            double newWidth = _windowStartSize.Width;
            double newHeight = _windowStartSize.Height;
            double newX = _windowStartPosition.X;
            double newY = _windowStartPosition.Y;

            if (_resizeDirection is ResizeDirection.Right or ResizeDirection.TopRight or ResizeDirection.BottomRight)
                newWidth += delta.X;
            if (_resizeDirection is ResizeDirection.Left or ResizeDirection.TopLeft or ResizeDirection.BottomLeft)
            {
                newWidth -= delta.X;
                newX += delta.X;
            }
            if (_resizeDirection is ResizeDirection.Bottom or ResizeDirection.BottomLeft or ResizeDirection.BottomRight)
                newHeight += delta.Y;
            if (_resizeDirection is ResizeDirection.Top or ResizeDirection.TopLeft or ResizeDirection.TopRight)
            {
                newHeight -= delta.Y;
                newY += delta.Y;
            }

            // Snap the size to the nearest increment
            double snappedWidth = Math.Round(newWidth / ResizeSnapIncrement) * ResizeSnapIncrement;
            double snappedHeight = Math.Round(newHeight / ResizeSnapIncrement) * ResizeSnapIncrement;

            double widthDiff = snappedWidth - newWidth;
            double heightDiff = snappedHeight - newHeight;

            // Adjust position to keep the non-dragged edge stationary during snapping
            if (_resizeDirection is ResizeDirection.Left or ResizeDirection.TopLeft or ResizeDirection.BottomLeft)
                newX -= widthDiff;
            if (_resizeDirection is ResizeDirection.Top or ResizeDirection.TopLeft or ResizeDirection.TopRight)
                newY -= heightDiff;
            
            newWidth = snappedWidth;
            newHeight = snappedHeight;

            const double minSize = 120;
            if (newWidth < minSize)
            {
                if (_resizeDirection is ResizeDirection.Left or ResizeDirection.TopLeft or ResizeDirection.BottomLeft)
                    newX -= (minSize - newWidth);
                newWidth = minSize;
            }
            if (newHeight < minSize)
            {
                if (_resizeDirection is ResizeDirection.Top or ResizeDirection.TopLeft or ResizeDirection.TopRight)
                    newY -= (minSize - newHeight);
                newHeight = minSize;
            }

            this.Width = newWidth;
            this.Height = newHeight;
            this.Position = new PixelPoint((int)newX, (int)newY);
            
            var sizeDisplay = this.FindControl<Border>("SizeDisplay");
            if (sizeDisplay != null)
            {
                if (sizeDisplay.Child is TextBlock textBlock)
                    textBlock.Text = $"{(int)newWidth} x {(int)newHeight}";
            }
        }
        else if (_isDragging && e.Pointer.Captured == this)
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
        else
        {
            var point = e.GetPosition(this);
            var direction = GetResizeDirection(point);
            this.Cursor = GetCursorForDirection(direction);
        }
    }

    private void Window_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (_isDragging || _isResizing)
        {
            _isDragging = false;
            _isResizing = false;
            e.Pointer.Capture(null);
            this.Cursor = Cursor.Default;

            var sizeDisplay = this.FindControl<Border>("SizeDisplay");
            if (sizeDisplay != null) sizeDisplay.IsVisible = false;
        }
    }

    private ResizeDirection GetResizeDirection(Point point)
    {
        const int resizeMargin = 8;
        bool onLeft = point.X < resizeMargin;
        bool onRight = point.X > this.Bounds.Width - resizeMargin;
        bool onTop = point.Y < resizeMargin;
        bool onBottom = point.Y > this.Bounds.Height - resizeMargin;

        if (onTop && onLeft) return ResizeDirection.TopLeft;
        if (onTop && onRight) return ResizeDirection.TopRight;
        if (onBottom && onLeft) return ResizeDirection.BottomLeft;
        if (onBottom && onRight) return ResizeDirection.BottomRight;
        if (onTop) return ResizeDirection.Top;
        if (onBottom) return ResizeDirection.Bottom;
        if (onLeft) return ResizeDirection.Left;
        if (onRight) return ResizeDirection.Right;

        return ResizeDirection.None;
    }

    private Cursor GetCursorForDirection(ResizeDirection direction)
    {
        return direction switch
        {
            ResizeDirection.Top or ResizeDirection.Bottom => new Cursor(StandardCursorType.SizeNorthSouth),
            ResizeDirection.Left or ResizeDirection.Right => new Cursor(StandardCursorType.SizeWestEast),
            ResizeDirection.TopLeft or ResizeDirection.BottomRight => new Cursor(StandardCursorType.TopLeftCorner),
            ResizeDirection.TopRight or ResizeDirection.BottomLeft => new Cursor(StandardCursorType.TopRightCorner),
            _ => Cursor.Default
        };
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
