using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Personal_Budgeting.Views.Pages;

public partial class LoginPage : UserControl
{
    public LoginPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}