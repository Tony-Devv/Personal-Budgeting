using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PersonalBudgeting.Views.Pages;

public partial class SettingsPage : UserControl
{
    public SettingsPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}