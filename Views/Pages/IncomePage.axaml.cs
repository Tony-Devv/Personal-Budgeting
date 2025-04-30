using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Personal_Budgeting.Views.Pages;

public partial class IncomePage : UserControl
{
    public IncomePage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}