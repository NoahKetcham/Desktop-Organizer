using Boxes.Models;
using Boxes.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Boxes.Pages;

public sealed partial class TemplatesPage : Page
{
    public TemplatesViewModel ViewModel { get; }

    public TemplatesPage()
    {
        InitializeComponent();
        ViewModel = App.Current.Services.GetRequiredService<TemplatesViewModel>();
    }

    private async void TemplateCard_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is OrganizationSetup setup)
        {
            await ViewModel.SelectSetupCommand.ExecuteAsync(setup);
        }
    }
}
