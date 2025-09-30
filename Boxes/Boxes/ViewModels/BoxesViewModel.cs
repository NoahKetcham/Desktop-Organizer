using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Boxes.Models;
using Boxes.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace Boxes.ViewModels;

public partial class BoxesViewModel : ObservableObject
{
    private readonly BoxManager _boxManager;

    [ObservableProperty]
    private string _newBoxName = "New Box";

    public ObservableCollection<Box> Boxes => _boxManager.Boxes;

    public BoxesViewModel(BoxManager boxManager)
    {
        _boxManager = boxManager;
    }

    [RelayCommand]
    private void CreateNewBox()
    {
        var box = _boxManager.CreateBox(_newBoxName);
        _boxManager.ShowBox(box);
        
        // Reset name for next box
        NewBoxName = "New Box";
    }

    [RelayCommand]
    private void ShowBox(Box box)
    {
        _boxManager.ShowBox(box);
    }

    [RelayCommand]
    private void HideBox(Box box)
    {
        _boxManager.HideBox(box);
    }

    [RelayCommand]
    private void DeleteBox(Box box)
    {
        _boxManager.DeleteBox(box);
    }

    [RelayCommand]
    private void ShowAllBoxes()
    {
        _boxManager.ShowAllBoxes();
    }

    [RelayCommand]
    private void HideAllBoxes()
    {
        _boxManager.HideAllBoxes();
    }

    [RelayCommand]
    private void EditBox(Box box)
    {
        // TODO: Show edit dialog
    }
}

