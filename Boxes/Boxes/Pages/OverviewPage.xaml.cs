using Boxes.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Boxes.Pages;

public sealed partial class OverviewPage : Page
{
    public MainViewModel ViewModel { get; }

    public OverviewPage()
    {
        InitializeComponent();
        ViewModel = App.Current.Services.GetRequiredService<MainViewModel>();
    }

    private void BrowseTemplates_Click(object sender, RoutedEventArgs e)
    {
        // Navigate to Setup Library page
        if (Parent is Frame frame)
        {
            frame.Navigate(typeof(TemplatesPage));
        }
    }
}
