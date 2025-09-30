using Boxes.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Boxes.Pages;

public sealed partial class DesktopFilesPage : Page
{
    public DesktopFilesViewModel ViewModel { get; }

    public DesktopFilesPage()
    {
        InitializeComponent();
        ViewModel = App.Current.Services.GetRequiredService<DesktopFilesViewModel>();
    }

    private Visibility ShowEmptyState(int totalFiles, bool isScanning)
    {
        return totalFiles == 0 && !isScanning ? Visibility.Visible : Visibility.Collapsed;
    }

    private Visibility ShowFileList(int totalFiles, bool isScanning)
    {
        return totalFiles > 0 && !isScanning ? Visibility.Visible : Visibility.Collapsed;
    }
}
