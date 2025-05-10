using System;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Controller;

namespace PersonalBudgeting.Views.Pages;

public partial class WelcomePage : UserControl
{
    private readonly UserController _userController;
    private readonly ContentControl _contentFrame;
    private readonly Action<int> _onLoginSuccess;

    public WelcomePage(UserController userController, ContentControl contentFrame, Action<int> onLoginSuccess)
    {
        InitializeComponent();
        _userController = userController;
        _contentFrame = contentFrame;
        _onLoginSuccess = onLoginSuccess;
    }

    private void OnLoginClick(object? sender, RoutedEventArgs e)
    {
        _contentFrame.Content = new LoginPage(_userController, _contentFrame, _onLoginSuccess);
    }

    private void OnSignUpClick(object? sender, RoutedEventArgs e)
    {
        _contentFrame.Content = new SignUpPage(_userController, _contentFrame, _onLoginSuccess);
    }
}
