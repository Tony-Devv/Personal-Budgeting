using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Controller;
using PersonalBudgeting.Views;

namespace PersonalBudgeting.Views.Pages;

public partial class WelcomePage : UserControl
{
    private readonly UserController _userController;
    private readonly ContentControl _pageContent;
    private readonly Action<int> _onLoginSuccess;

    public WelcomePage(UserController userController, ContentControl pageContent, Action<int> onLoginSuccess)
    {
        InitializeComponent();
        _userController = userController;
        _pageContent = pageContent;
        _onLoginSuccess = onLoginSuccess;
    }

    private void OnLoginClick(object? sender, RoutedEventArgs e)
    {
        _pageContent.Content = new LoginPage(_userController, _pageContent, _onLoginSuccess);
    }

    private void OnSignUpClick(object? sender, RoutedEventArgs e)
    {
        _pageContent.Content = new SignUpPage(_userController, _pageContent, _onLoginSuccess);
    }
}
