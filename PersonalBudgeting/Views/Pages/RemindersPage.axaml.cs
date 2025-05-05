using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PersonalBudgeting.Views.Pages;

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