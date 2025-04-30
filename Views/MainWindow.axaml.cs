using Avalonia.Controls;
using FluentAvalonia.UI.Controls;
using Personal_Budgeting.Views.Pages;

namespace Personal_Budgeting.Views;

public partial class MainWindow : Window
{
    private readonly Frame _contentFrame;

    public MainWindow()
    {
        InitializeComponent();
        
        _contentFrame = this.FindControl<Frame>("ContentFrame");
        var navView = this.FindControl<NavigationView>("NavView");
        
        if (navView != null)
        {
            navView.SelectionChanged += NavView_SelectionChanged;
        }

        // Navigate to home page by default
        if (_contentFrame != null)
        {
            _contentFrame.Navigate(typeof(HomePage));
        }
    }

    private void NavView_SelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e)
    {
        if (e.SelectedItem is NavigationViewItem item && _contentFrame != null)
        {
            switch (item.Tag?.ToString()?.ToLower())
            {
                case "home":
                    _contentFrame.Navigate(typeof(HomePage));
                    break;
                case "profile":
                    _contentFrame.Navigate(typeof(ProfilePage));
                    break;
                case "income":
                    _contentFrame.Navigate(typeof(IncomePage));
                    break;
                case "expenses":
                    _contentFrame.Navigate(typeof(ExpensesPage));
                    break;
                case "budget":
                    _contentFrame.Navigate(typeof(BudgetPage));
                    break;
                case "reminders":
                    _contentFrame.Navigate(typeof(RemindersPage));
                    break;
                case "settings":
                    _contentFrame.Navigate(typeof(SettingsPage));
                    break;
            }
        }
    }
}