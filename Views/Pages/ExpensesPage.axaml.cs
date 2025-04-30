using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace Personal_Budgeting.Views.Pages;

public partial class ExpensesPage : UserControl
{
    public ExpensesPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}