using Boxes.Models;
using Boxes.Services;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Windows.UI;
using Color = Windows.UI.Color;
using Brush = Microsoft.UI.Xaml.Media.Brush;
using SolidColorBrush = Microsoft.UI.Xaml.Media.SolidColorBrush;

namespace Boxes.Pages;

public sealed partial class BoxesPage : Page
{
    private readonly BoxManager _boxManager;

    public BoxesPage()
    {
        InitializeComponent();
        _boxManager = App.Current.Services.GetRequiredService<BoxManager>();

        // Bind to boxes collection
        BoxesItemsControl.ItemsSource = _boxManager.Boxes;

        // Listen for box changes to update visual states
        _boxManager.Boxes.CollectionChanged += Boxes_CollectionChanged;
    }

    private void Boxes_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        // Update visual states when boxes are added/removed
        UpdateAllBoxVisualStates();
    }

    private void UpdateAllBoxVisualStates()
    {
        // This would be called when boxes are added/removed to update visual states
        // For now, we'll handle it in the item template binding
    }


    private void AddNewBoxButton_Click(object sender, RoutedEventArgs e)
    {
        var boxName = string.IsNullOrWhiteSpace(NewBoxNameTextBox.Text)
            ? "New Box"
            : NewBoxNameTextBox.Text;

        var selectedStyle = (BoxStyle)StyleComboBox.SelectedIndex;
        var selectedColor = GetSelectedColor();
        var opacity = OpacitySlider.Value / 100.0;

        var box = _boxManager.CreateBox(boxName, selectedStyle, opacity);
        box.Color = selectedColor;
        _boxManager.ShowBox(box);

        // Reset controls
        NewBoxNameTextBox.Text = "My New Box";
        StyleComboBox.SelectedIndex = 0;
        ColorComboBox.SelectedIndex = 0;
        OpacitySlider.Value = 95;
    }

    private void PreviewToggleButton_Checked(object sender, RoutedEventArgs e)
    {
        UpdatePreviewBox();
    }

    private void PreviewToggleButton_Unchecked(object sender, RoutedEventArgs e)
    {
        // Hide preview when toggled off
        PreviewBoxBorder.Visibility = Visibility.Collapsed;
    }

    private void OpacitySlider_ValueChanged(object sender, Microsoft.UI.Xaml.Controls.Primitives.RangeBaseValueChangedEventArgs e)
    {
        if (PreviewToggleButton.IsChecked == true)
        {
            UpdatePreviewBox();
        }
    }

    private void UpdatePreviewBox()
    {
        if (PreviewToggleButton.IsChecked != true) return;

        // Show preview box
        PreviewBoxBorder.Visibility = Visibility.Visible;

        // Update preview box appearance based on current settings
        var selectedStyle = (BoxStyle)StyleComboBox.SelectedIndex;
        var selectedColor = GetSelectedColor();
        var opacity = OpacitySlider.Value / 100.0;

        // Update preview box styling
        var borderBrush = new SolidColorBrush(ParseColor(selectedColor));
        PreviewBoxBorder.BorderBrush = borderBrush;

        // Update background based on style
        var backgroundBrush = GetPreviewBackgroundBrush(selectedStyle, selectedColor, opacity);
        PreviewBoxBorder.Background = backgroundBrush;

        // Update text
        var styleName = ((ComboBoxItem)StyleComboBox.SelectedItem).Content.ToString();
        PreviewBoxBorder.Child = new TextBlock
        {
            Text = $"📦 {NewBoxNameTextBox.Text}\n{styleName}",
            FontSize = 12,
            Foreground = new SolidColorBrush(ParseColor(selectedColor)),
            HorizontalAlignment = HorizontalAlignment.Center,
            VerticalAlignment = VerticalAlignment.Center,
            TextAlignment = TextAlignment.Center
        };
    }

    private string GetSelectedColor()
    {
        var colors = new[]
        {
            "#0078D4", // Blue
            "#8764B8", // Purple
            "#00CC6A", // Green
            "#E81123", // Red
            "#FF8C00", // Orange
            "#00B7C3"  // Teal
        };

        return colors[ColorComboBox.SelectedIndex];
    }

    private Brush GetPreviewBackgroundBrush(BoxStyle style, string colorHex, double opacity)
    {
        var baseColor = ParseColor(colorHex);

        switch (style)
        {
            case BoxStyle.Windows:
                return new SolidColorBrush(Color.FromArgb((byte)(opacity * 255), 240, 240, 240));
            case BoxStyle.Acetate:
                return new SolidColorBrush(Color.FromArgb((byte)(opacity * 60), baseColor.R, baseColor.G, baseColor.B));
            case BoxStyle.Solid:
                return new SolidColorBrush(Color.FromArgb((byte)(opacity * 255), baseColor.R, baseColor.G, baseColor.B));
            case BoxStyle.Acrylic:
                return new SolidColorBrush(Color.FromArgb((byte)(opacity * 128), baseColor.R, baseColor.G, baseColor.B));
            case BoxStyle.Minimal:
                return new SolidColorBrush(Color.FromArgb((byte)(opacity * 40), baseColor.R, baseColor.G, baseColor.B));
            case BoxStyle.Frosted:
                return new SolidColorBrush(Color.FromArgb((byte)(opacity * 180), baseColor.R, baseColor.G, baseColor.B));
            default:
                return new SolidColorBrush(Color.FromArgb((byte)(opacity * 255), 240, 240, 240));
        }
    }

    private Color ParseColor(string colorHex)
    {
        if (string.IsNullOrEmpty(colorHex) || !colorHex.StartsWith("#"))
            return Color.FromArgb(255, 0, 120, 212); // Default blue

        try
        {
            var hex = colorHex.Substring(1);
            if (hex.Length == 6)
            {
                var r = byte.Parse(hex.Substring(0, 2), System.Globalization.NumberStyles.HexNumber);
                var g = byte.Parse(hex.Substring(2, 2), System.Globalization.NumberStyles.HexNumber);
                var b = byte.Parse(hex.Substring(4, 2), System.Globalization.NumberStyles.HexNumber);
                return Color.FromArgb(255, r, g, b);
            }
        }
        catch { }

        return Color.FromArgb(255, 0, 120, 212); // Default blue
    }

    private void ShowAllButton_Click(object sender, RoutedEventArgs e)
    {
        _boxManager.ShowAllBoxes();
    }

    private void HideAllButton_Click(object sender, RoutedEventArgs e)
    {
        _boxManager.HideAllBoxes();
    }

    private void ShowBoxButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is Box box)
        {
            if (box.IsVisible)
            {
                _boxManager.HideBox(box);
                UpdateButtonText(button, "Show on Desktop");
            }
            else
            {
                _boxManager.ShowBox(box);
                UpdateButtonText(button, "Hide from Desktop");
            }
        }
    }

    private void UpdateButtonText(Button button, string text)
    {
        // Find the button in the visual tree and update its content
        if (button.FindName("ShowHideButton") is Button showHideButton)
        {
            showHideButton.Content = text;
        }
    }

    private void EditBoxButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is Box box)
        {
            // TODO: Implement proper edit dialog
            var dialog = new ContentDialog
            {
                Title = $"Edit {box.Name}",
                Content = $"Edit functionality for '{box.Name}' coming soon!",
                CloseButtonText = "OK",
                XamlRoot = this.XamlRoot
            };
            _ = dialog.ShowAsync();
        }
    }

    private void DeleteBoxButton_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.DataContext is Box box)
        {
            var dialog = new ContentDialog
            {
                Title = "Delete Box",
                Content = $"Are you sure you want to delete '{box.Name}'? This action cannot be undone.",
                PrimaryButtonText = "Delete",
                CloseButtonText = "Cancel",
                DefaultButton = ContentDialogButton.Primary,
                XamlRoot = this.XamlRoot
            };

            // For now, just delete without confirmation to avoid async issues
            _boxManager.DeleteBox(box);
        }
    }
}
