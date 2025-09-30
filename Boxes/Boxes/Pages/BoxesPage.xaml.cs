using Boxes.Models;
using Boxes.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

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
    }

    private void AddNewBoxButton_Click(object sender, RoutedEventArgs e)
    {
        var boxName = string.IsNullOrWhiteSpace(NewBoxNameTextBox.Text) 
            ? "New Box" 
            : NewBoxNameTextBox.Text;
        
        var selectedStyle = (BoxStyle)StyleComboBox.SelectedIndex;
            
        var box = _boxManager.CreateBox(boxName, selectedStyle, 0.95);
        _boxManager.ShowBox(box);
        
        // Reset textbox
        NewBoxNameTextBox.Text = "My New Box";
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
        // TODO: Get the box from the button's data context
        // For now, show the first box as a demo
        if (_boxManager.Boxes.Count > 0)
        {
            _boxManager.ShowBox(_boxManager.Boxes[0]);
        }
    }

    private void EditBoxButton_Click(object sender, RoutedEventArgs e)
    {
        // TODO: Implement edit functionality
        var dialog = new ContentDialog
        {
            Title = "Edit Box",
            Content = "Edit functionality coming soon!",
            CloseButtonText = "OK",
            XamlRoot = this.XamlRoot
        };
        _ = dialog.ShowAsync();
    }

    private void DeleteBoxButton_Click(object sender, RoutedEventArgs e)
    {
        // TODO: Get the box from the button's data context and delete it
        // For now, show a message
        var dialog = new ContentDialog
        {
            Title = "Delete Box",
            Content = "Delete functionality coming soon!",
            CloseButtonText = "OK",
            XamlRoot = this.XamlRoot
        };
        _ = dialog.ShowAsync();
    }
}
