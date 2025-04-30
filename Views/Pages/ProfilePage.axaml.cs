using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Personal_Budgeting.Views.Pages;

public partial class ProfilePage : UserControl
{
    public ProfilePage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}