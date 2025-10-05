using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
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

    public Guid Id => _boxData.Id;

    public DesktopBox()
    {
        System.Diagnostics.Debug.WriteLine("DesktopBox: Constructor called");
        InitializeComponent();
        System.Diagnostics.Debug.WriteLine("DesktopBox: InitializeComponent completed");

        // Default box for testing
        _boxData = new Box
        {
            Id = Guid.NewGuid(),
            Name = "Desktop Box",
            Description = "A beautiful glass-like desktop container",
            Style = BoxStyle.Windows,
            X = 100,
            Y = 100,
            Width = 400,
            Height = 300,
            IsVisible = true,
            CreatedDate = DateTime.Now
        };

        System.Diagnostics.Debug.WriteLine("DesktopBox: About to call SetupWindow");
        SetupWindow();
        System.Diagnostics.Debug.WriteLine("DesktopBox: SetupWindow completed");
    }

    public DesktopBox(Box box)
    {
        InitializeComponent();
        _boxData = box ?? throw new ArgumentNullException(nameof(box));
        SetupWindow();
    }

    private void SetupWindow()
    {
        System.Diagnostics.Debug.WriteLine("DesktopBox: SetupWindow started");

        // Set window properties
        Title = _boxData.Name;
        Width = _boxData.Width;
        Height = _boxData.Height;
        Position = new PixelPoint((int)_boxData.X, (int)_boxData.Y);
        WindowStartupLocation = WindowStartupLocation.Manual;
        ShowInTaskbar = false;
        Topmost = true;
        CanResize = true;
        SystemDecorations = SystemDecorations.None;
        TransparencyLevelHint = [WindowTransparencyLevel.AcrylicBlur];
        Background = Brushes.Transparent;

        System.Diagnostics.Debug.WriteLine("DesktopBox: Window properties set, updating glass effect");

        // Update glass effect based on box style
        UpdateGlassEffect();

        System.Diagnostics.Debug.WriteLine("DesktopBox: SetupWindow completed");
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        // Subscribe to drag and drop events
        this.AddHandler(DragDrop.DropEvent, DropZone_Drop);
        this.AddHandler(DragDrop.DragOverEvent, DropZone_DragOver);

        // Subscribe to pointer events for dragging
        this.PointerPressed += DragArea_PointerPressed;
        this.PointerMoved += DragArea_PointerMoved;
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

            // Capture the mouse to receive events even if pointer leaves the window
            e.Pointer.Capture(this);
        }
    }

    private void DragArea_PointerMoved(object? sender, PointerEventArgs e)
    {
        if (_isDragging && e.Pointer.Captured == this)
        {
            var currentPoint = e.GetPosition(this);
            var deltaX = currentPoint.X - _dragStartPoint.X;
            var deltaY = currentPoint.Y - _dragStartPoint.Y;

            var newX = _windowStartPosition.X + deltaX;
            var newY = _windowStartPosition.Y + deltaY;

            // Keep window on screen bounds
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

    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        this.Close();
    }

    private void DropZone_DragOver(object? sender, DragEventArgs e)
    {
        // Check if drag data contains files using the newer API
        if (e.Data.GetDataFormats().Contains("File"))
        {
            e.DragEffects = DragDropEffects.Copy;
        }
        else
        {
            e.DragEffects = DragDropEffects.None;
        }
    }

    private void DropZone_Drop(object? sender, DragEventArgs e)
    {
        // Handle file drop using the newer API
        if (e.Data.GetDataFormats().Contains("File"))
        {
            var files = e.Data.GetFiles();
            if (files != null)
            {
                foreach (var file in files)
                {
                    // TODO: Handle file drop - add to box, organize, etc.
                    Console.WriteLine($"File dropped: {file.Name}");
                }
            }
        }
    }

    #endregion

    #region Window Events

    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);

        // Additional initialization when window opens
        UpdateGlassEffect();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        // Cleanup when window closes
    }

    #endregion

    #region Glass Effect Management

    private void UpdateGlassEffect()
    {
        try
        {
            // Find the ExperimentalAcrylicBorder control
            var glassBorder = this.FindControl<ExperimentalAcrylicBorder>("GlassBorder");
            if (glassBorder != null)
            {
                // Apply glass effect based on the box style
                switch (_boxData.Style)
                {
                    case BoxStyle.Windows:
                        glassBorder.Material = new ExperimentalAcrylicMaterial
                        {
                            BackgroundSource = AcrylicBackgroundSource.Digger,
                            TintColor = Color.FromArgb(255, 0, 120, 212), // Windows blue
                            TintOpacity = 0.15,
                            MaterialOpacity = 0.8
                        };
                        break;

                    case BoxStyle.Acetate:
                        glassBorder.Material = new ExperimentalAcrylicMaterial
                        {
                            BackgroundSource = AcrylicBackgroundSource.Digger,
                            TintColor = Color.FromArgb(255, 50, 50, 50), // Dark tint
                            TintOpacity = 0.2,
                            MaterialOpacity = 0.7
                        };
                        break;

                    case BoxStyle.Acrylic:
                        glassBorder.Material = new ExperimentalAcrylicMaterial
                        {
                            BackgroundSource = AcrylicBackgroundSource.Digger,
                            TintColor = Color.FromArgb(255, 255, 255, 255), // White tint
                            TintOpacity = 0.1,
                            MaterialOpacity = 0.9
                        };
                        break;

                    case BoxStyle.Frosted:
                        glassBorder.Material = new ExperimentalAcrylicMaterial
                        {
                            BackgroundSource = AcrylicBackgroundSource.Digger,
                            TintColor = Color.FromArgb(255, 200, 200, 200), // Light gray
                            TintOpacity = 0.25,
                            MaterialOpacity = 0.6
                        };
                        break;

                    case BoxStyle.Minimal:
                        glassBorder.Material = new ExperimentalAcrylicMaterial
                        {
                            BackgroundSource = AcrylicBackgroundSource.Digger,
                            TintColor = Color.FromArgb(255, 0, 0, 0), // Black tint
                            TintOpacity = 0.05,
                            MaterialOpacity = 0.95
                        };
                        break;

                    default:
                        glassBorder.Material = new ExperimentalAcrylicMaterial
                        {
                            BackgroundSource = AcrylicBackgroundSource.Digger,
                            TintColor = Color.FromArgb(255, 0, 120, 212), // Default Windows blue
                            TintOpacity = 0.15,
                            MaterialOpacity = 0.8
                        };
                        break;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error updating glass effect: {ex.Message}");
        }
    }

    public void ApplyBoxStyle(BoxStyle style)
    {
        _boxData.Style = style;
        UpdateGlassEffect();
    }

    #endregion
}
