using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Personal_Budgeting.Views.Pages;

public partial class RemindersPage : UserControl
{
    public RemindersPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}