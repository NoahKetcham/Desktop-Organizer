using System;
using Boxes.Pages;
using Boxes.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Boxes;

/// <summary>
/// An empty window that can be used on its own or navigated to within a Frame.
/// </summary>
public sealed partial class MainWindow : Window
{
    public MainViewModel ViewModel { get; }

    public MainWindow()
    {
        InitializeComponent();

        // Get ViewModel from DI
        ViewModel = App.Current.Services.GetRequiredService<MainViewModel>();
        
        // Set window properties
        Title = ViewModel.Title;
        ExtendsContentIntoTitleBar = true;
        SetTitleBar(AppTitleBar);

        // Navigate to Overview page by default
        ContentFrame.Navigate(typeof(OverviewPage));
    }

    private void NavView_SelectionChanged(NavigationView sender, NavigationViewSelectionChangedEventArgs args)
    {
        if (args.SelectedItem is NavigationViewItem item && item.Tag is string tag)
        {
            Type? pageType = tag switch
            {
                "Overview" => typeof(OverviewPage),
                "SetupLibrary" => typeof(TemplatesPage),
                "OrganizationMethods" => typeof(OrganizationMethodsPage),
                "DesktopFiles" => typeof(DesktopFilesPage),
                "Boxes" => typeof(BoxesPage),
                "Categories" => null, // TODO: Create this page
                "Rules" => null, // TODO: Create this page
                "History" => null, // TODO: Create this page
                _ => null
            };

            if (pageType != null)
            {
                ContentFrame.Navigate(pageType);
            }
        }
    }
}
