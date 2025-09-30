using System;
using Boxes.Models;
using Boxes.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Boxes.Pages;

public sealed partial class OrganizationMethodsPage : Page
{
    public OrganizationMethodsViewModel ViewModel { get; }

    public OrganizationMethodsPage()
    {
        InitializeComponent();
        ViewModel = App.Current.Services.GetRequiredService<OrganizationMethodsViewModel>();
    }

    private async void OrganizationMethodCard_Click(Controls.OrganizationMethodCard sender, OrganizationMethod method)
    {
        // Show details dialog or configure the method
        ContentDialog dialog = new ContentDialog
        {
            Title = method.Name,
            Content = $"Configure {method.Name}\n\nThis method will organize {method.FilesAffected} files.\n\nMethod type: {method.Type}",
            PrimaryButtonText = "Apply Now",
            CloseButtonText = "Cancel",
            XamlRoot = XamlRoot
        };

        var result = await dialog.ShowAsync();
        
        if (result == ContentDialogResult.Primary)
        {
            // TODO: Apply the organization method
        }
    }
}
