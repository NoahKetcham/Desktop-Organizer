using Boxes.Models;
using Microsoft.UI.Xaml.Controls;

namespace Boxes.Dialogs;

public sealed partial class TemplateDetailsDialog : ContentDialog
{
    public OrganizationSetup Setup { get; }

    public TemplateDetailsDialog(OrganizationSetup setup)
    {
        Setup = setup;
        InitializeComponent();
    }
}
