using Boxes.Models;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace Boxes.Controls;

public sealed partial class OrganizationMethodCard : UserControl
{
    public static readonly DependencyProperty MethodProperty =
        DependencyProperty.Register(
            nameof(Method),
            typeof(OrganizationMethod),
            typeof(OrganizationMethodCard),
            new PropertyMetadata(null));

    public OrganizationMethod Method
    {
        get => (OrganizationMethod)GetValue(MethodProperty);
        set => SetValue(MethodProperty, value);
    }

    public event TypedEventHandler<OrganizationMethodCard, OrganizationMethod>? CardClicked;

    public OrganizationMethodCard()
    {
        InitializeComponent();
    }

    private void CardButton_Click(object sender, RoutedEventArgs e)
    {
        CardClicked?.Invoke(this, Method);
    }

    private string GetMethodTypeText(OrganizationMethodType type)
    {
        return type switch
        {
            OrganizationMethodType.ByFileType => "Organize by File Type",
            OrganizationMethodType.ByDate => "Organize by Date",
            OrganizationMethodType.BySize => "Organize by Size",
            OrganizationMethodType.ByName => "Organize by Name",
            OrganizationMethodType.ByProject => "Organize by Project",
            OrganizationMethodType.Custom => "Custom Method",
            _ => "Organization Method"
        };
    }
}
