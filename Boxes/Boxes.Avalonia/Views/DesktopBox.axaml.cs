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

    // When true, we force fully transparent content backgrounds to see desktop.
    // You can toggle this at runtime if you ever add a “glass vs. clear” switch.
    private bool _seeThroughEnabled = true;

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

        // No system chrome; we draw our own frame
        SystemDecorations = SystemDecorations.None;

        // Request per-pixel transparency from OS compositor
        TransparencyLevelHint = new[] { WindowTransparencyLevel.Transparent };

        // The window surface itself must be transparent
        Background = Brushes.Transparent;

        System.Diagnostics.Debug.WriteLine("DesktopBox: Window properties set, updating acetate effect");

        // Update acetate effect based on box style
        UpdateAcetateEffect();

        System.Diagnostics.Debug.WriteLine("DesktopBox: SetupWindow completed");
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);

        // Subscribe to drag and drop events
        this.AddHandler(DragDrop.DropEvent, DropZone_Drop);
        this.AddHandler(DragDrop.DragOverEvent, DropZone_DragOver);

        // Subscribe to pointer events for dragging (custom hit area)
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
        // Keep your existing logic; Avalonia’s cross-platform formats vary by platform.
        // If needed, you can also check DataFormats.FileNames for compatibility.
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
        UpdateAcetateEffect();
    }

    protected override void OnClosed(EventArgs e)
    {
        base.OnClosed(e);

        // Cleanup when window closes
    }

    #endregion

    #region Acetate Effect Management

    private void UpdateAcetateEffect()
    {
        try
        {
            // Find the main acetate border control
            var acetateBorder = this.FindControl<Border>("AcetateBorder");
            if (acetateBorder != null)
            {
                // IMPORTANT:
                // For true see-through to desktop, DO NOT paint an opaque/blurred background.
                // We keep the border accents per style, but force the Background to Transparent
                // when _seeThroughEnabled is true.
                switch (_boxData.Style)
                {
                    case BoxStyle.Acetate:
                        acetateBorder.BorderBrush = new LinearGradientBrush
                        {
                            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                            EndPoint   = new RelativePoint(1, 1, RelativeUnit.Relative),
                            GradientStops =
                            {
                                new GradientStop(Color.Parse("#80C0E0F0"), 0.0),
                                new GradientStop(Color.Parse("#60D0E8F6"), 0.25),
                                new GradientStop(Color.Parse("#60D0E8F6"), 0.75),
                                new GradientStop(Color.Parse("#40E0F0FF"), 1.0)
                            }
                        };
                        acetateBorder.Background = _seeThroughEnabled
                            ? Brushes.Transparent
                            : new LinearGradientBrush
                            {
                                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                                EndPoint   = new RelativePoint(1, 1, RelativeUnit.Relative),
                                GradientStops =
                                {
                                    new GradientStop(Color.Parse("#E8F4FD"), 0.0),
                                    new GradientStop(Color.Parse("#F0F8FF"), 0.3),
                                    new GradientStop(Color.Parse("#E6F3FF"), 0.7),
                                    new GradientStop(Color.Parse("#D1E9F6"), 1.0)
                                }
                            };
                        break;

                    case BoxStyle.Windows:
                        acetateBorder.BorderBrush = new LinearGradientBrush
                        {
                            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                            EndPoint   = new RelativePoint(1, 1, RelativeUnit.Relative),
                            GradientStops =
                            {
                                new GradientStop(Color.FromArgb(128, 0, 120, 212), 0.0),
                                new GradientStop(Color.FromArgb(96, 0, 140, 240), 0.25),
                                new GradientStop(Color.FromArgb(96, 0, 140, 240), 0.75),
                                new GradientStop(Color.FromArgb(64, 0, 160, 255), 1.0)
                            }
                        };
                        acetateBorder.Background = _seeThroughEnabled
                            ? Brushes.Transparent
                            : new LinearGradientBrush
                            {
                                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                                EndPoint   = new RelativePoint(1, 1, RelativeUnit.Relative),
                                GradientStops =
                                {
                                    new GradientStop(Color.FromArgb(255, 232, 244, 253), 0.0),
                                    new GradientStop(Color.FromArgb(255, 240, 248, 255), 0.3),
                                    new GradientStop(Color.FromArgb(255, 230, 243, 255), 0.7),
                                    new GradientStop(Color.FromArgb(255, 209, 233, 246), 1.0)
                                }
                            };
                        break;

                    case BoxStyle.Acrylic:
                        acetateBorder.BorderBrush = new LinearGradientBrush
                        {
                            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                            EndPoint   = new RelativePoint(1, 1, RelativeUnit.Relative),
                            GradientStops =
                            {
                                new GradientStop(Color.FromArgb(128, 255, 255, 255), 0.0),
                                new GradientStop(Color.FromArgb(96, 240, 240, 240), 0.25),
                                new GradientStop(Color.FromArgb(96, 240, 240, 240), 0.75),
                                new GradientStop(Color.FromArgb(64, 220, 220, 220), 1.0)
                            }
                        };
                        acetateBorder.Background = _seeThroughEnabled
                            ? Brushes.Transparent
                            : new LinearGradientBrush
                            {
                                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                                EndPoint   = new RelativePoint(1, 1, RelativeUnit.Relative),
                                GradientStops =
                                {
                                    new GradientStop(Color.FromArgb(255, 248, 252, 253), 0.0),
                                    new GradientStop(Color.FromArgb(255, 250, 254, 255), 0.3),
                                    new GradientStop(Color.FromArgb(255, 246, 251, 255), 0.7),
                                    new GradientStop(Color.FromArgb(255, 241, 247, 250), 1.0)
                                }
                            };
                        break;

                    case BoxStyle.Frosted:
                        acetateBorder.BorderBrush = new LinearGradientBrush
                        {
                            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                            EndPoint   = new RelativePoint(1, 1, RelativeUnit.Relative),
                            GradientStops =
                            {
                                new GradientStop(Color.FromArgb(128, 200, 200, 200), 0.0),
                                new GradientStop(Color.FromArgb(96, 220, 220, 220), 0.25),
                                new GradientStop(Color.FromArgb(96, 220, 220, 220), 0.75),
                                new GradientStop(Color.FromArgb(64, 240, 240, 240), 1.0)
                            }
                        };
                        acetateBorder.Background = _seeThroughEnabled
                            ? Brushes.Transparent
                            : new LinearGradientBrush
                            {
                                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                                EndPoint   = new RelativePoint(1, 1, RelativeUnit.Relative),
                                GradientStops =
                                {
                                    new GradientStop(Color.FromArgb(255, 234, 244, 253), 0.0),
                                    new GradientStop(Color.FromArgb(255, 242, 250, 255), 0.3),
                                    new GradientStop(Color.FromArgb(255, 238, 247, 255), 0.7),
                                    new GradientStop(Color.FromArgb(255, 225, 241, 248), 1.0)
                                }
                            };
                        break;

                    case BoxStyle.Minimal:
                        acetateBorder.BorderBrush = new LinearGradientBrush
                        {
                            StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                            EndPoint   = new RelativePoint(1, 1, RelativeUnit.Relative),
                            GradientStops =
                            {
                                new GradientStop(Color.FromArgb(128, 0, 0, 0), 0.0),
                                new GradientStop(Color.FromArgb(96, 50, 50, 50), 0.25),
                                new GradientStop(Color.FromArgb(96, 50, 50, 50), 0.75),
                                new GradientStop(Color.FromArgb(64, 100, 100, 100), 1.0)
                            }
                        };
                        acetateBorder.Background = _seeThroughEnabled
                            ? Brushes.Transparent
                            : new LinearGradientBrush
                            {
                                StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                                EndPoint   = new RelativePoint(1, 1, RelativeUnit.Relative),
                                GradientStops =
                                {
                                    new GradientStop(Color.FromArgb(255, 248, 248, 248), 0.0),
                                    new GradientStop(Color.FromArgb(255, 250, 250, 250), 0.3),
                                    new GradientStop(Color.FromArgb(255, 246, 246, 246), 0.7),
                                    new GradientStop(Color.FromArgb(255, 241, 241, 241), 1.0)
                                }
                            };
                        break;
                }

                // Ensure the *container* itself remains see-through
                acetateBorder.Background ??= Brushes.Transparent;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error updating acetate effect: {ex.Message}");
        }
    }

    public void ApplyBoxStyle(BoxStyle style)
    {
        _boxData.Style = style;
        UpdateAcetateEffect();
    }

    #endregion
}
