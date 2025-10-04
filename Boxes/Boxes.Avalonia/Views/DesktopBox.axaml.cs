using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.Threading;
using Boxes.Models;
using System;

namespace Boxes.Avalonia.Views;

public partial class DesktopBox : Window
{
    private readonly Box _boxData;
    private Point _dragStartPosition;

    public DesktopBox()
    {
        System.Diagnostics.Debug.WriteLine("DesktopBox: Constructor called");
        InitializeComponent();
        System.Diagnostics.Debug.WriteLine("DesktopBox: InitializeComponent completed");

        // Default box for testing
        _boxData = new Box
        {
            Id = Guid.NewGuid(),
            Name = "Sample Desktop Box",
            Description = "A beautiful glass-like desktop container",
            Style = BoxStyle.Acetate,
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
        _boxData = box;
        SetupWindow();
    }

    private void SetupWindow()
    {
        System.Diagnostics.Debug.WriteLine("DesktopBox: SetupWindow started");
        Title = _boxData.Name;
        Width = _boxData.Width;
        Height = _boxData.Height;
        Position = new PixelPoint((int)_boxData.X, (int)_boxData.Y);
        SystemDecorations = SystemDecorations.None;
        Background = Brushes.Transparent;

        System.Diagnostics.Debug.WriteLine("DesktopBox: Window properties set, applying glass effect");

        // Apply glass effect after a short delay to ensure XAML is fully loaded
        Dispatcher.UIThread.Post(() =>
        {
            System.Diagnostics.Debug.WriteLine("DesktopBox: Dispatcher callback executing");
            var glassBorder = this.FindControl<Border>("GlassBorder");
            System.Diagnostics.Debug.WriteLine($"DesktopBox: GlassBorder found: {glassBorder != null}");

            if (glassBorder != null)
            {
                System.Diagnostics.Debug.WriteLine("DesktopBox: Applying glass effect");
                Services.GlassEffectService.ApplyGlassEffect(glassBorder);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("DesktopBox: GlassBorder is null, cannot apply glass effect");
            }
        });

        System.Diagnostics.Debug.WriteLine("DesktopBox: SetupWindow completed");
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    protected override void OnPointerPressed(PointerPressedEventArgs e)
    {
        if (e.GetCurrentPoint(this).Properties.IsLeftButtonPressed)
        {
            _dragStartPosition = e.GetPosition(this);
            BeginMoveDrag(e);
        }
        base.OnPointerPressed(e);
    }

    protected override void OnPointerReleased(PointerReleasedEventArgs e)
    {
        base.OnPointerReleased(e);
    }

    private void CloseButton_Click(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void DragArea_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        BeginMoveDrag(e);
    }
}
