using System;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Boxes.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _title = "Desktop Organizer";

    [ObservableProperty]
    private string _welcomeMessage = "Welcome to Boxes";

    public MainViewModel()
    {
        // Initialize overview dashboard data
    }

    [RelayCommand]
    private void RefreshDesktop()
    {
        // TODO: Implement desktop refresh logic
        WelcomeMessage = $"Refreshed at {DateTime.Now:T}";
    }
}
