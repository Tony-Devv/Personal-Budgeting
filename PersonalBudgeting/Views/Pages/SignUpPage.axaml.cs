using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Controller;
using Model.Entities;
using PersonalBudgeting.Views;

namespace PersonalBudgeting.Views.Pages;

public partial class SignUpPage : UserControl
{
    private readonly UserController _userController;
    private readonly ContentControl _contentFrame;
    private readonly Action<int> _onLoginSuccess;

    // UI elements
    private TextBox _usernameInput;
    private TextBox _emailInput;
    private TextBox _passwordInput;
    private TextBox _phoneInput;
    private TextBlock _registerErrorText;

    public SignUpPage(UserController userController, ContentControl contentFrame, Action<int> onLoginSuccess)
    {
        InitializeComponent();
        _userController = userController;
        _contentFrame = contentFrame;
        _onLoginSuccess = onLoginSuccess;

        // Initialize UI elements safely
        _usernameInput = this.FindControl<TextBox>("UsernameInput");
        _emailInput = this.FindControl<TextBox>("EmailInput");
        _passwordInput = this.FindControl<TextBox>("PasswordInput");
        _phoneInput = this.FindControl<TextBox>("PhoneInput");
        _registerErrorText = this.FindControl<TextBlock>("RegisterErrorText");
        
        if (_registerErrorText != null)
            _registerErrorText.IsVisible = false;
    }

    private void OnBackClick(object? sender, RoutedEventArgs e)
    {
        _contentFrame.Content = new WelcomePage(_userController, _contentFrame, _onLoginSuccess);
    }

    private void OnLoginLinkClick(object? sender, RoutedEventArgs e)
    {
        _contentFrame.Content = new LoginPage(_userController, _contentFrame, _onLoginSuccess);
    }

    private async void OnRegisterClick(object? sender, RoutedEventArgs e)
    {
        // Check if UI elements are initialized
        if (_usernameInput == null || _emailInput == null || _passwordInput == null)
        {
            ShowError("UI elements not initialized properly");
            return;
        }

        // Get input values and validate
        var username = _usernameInput.Text?.Trim();
        var email = _emailInput.Text?.Trim();
        var password = _passwordInput.Text?.Trim();
        var phone = _phoneInput?.Text?.Trim();

        // Basic validation
        if (string.IsNullOrWhiteSpace(username))
        {
            ShowError("Username is required");
            return;
        }

        if (string.IsNullOrWhiteSpace(email))
        {
            ShowError("Email is required");
            return;
        }

        if (string.IsNullOrWhiteSpace(password))
        {
            ShowError("Password is required");
            return;
        }

        if (password.Length < 6)
        {
            ShowError("Password must be at least 6 characters");
            return;
        }

        try
        {
            // Create user object
            var newUser = new User
            {
                UserName = username,
                Email = email ?? string.Empty,
                Password = password,
                PhoneNumber = phone
            };

            // Save to database
            var (success, registeredUser, errors) = await _userController.TryAddUser(newUser);

            if (success && registeredUser != null)
            {
                // Registration successful
                _onLoginSuccess(registeredUser.Id);
            }
            else
            {
                // Show error from validation
                var errorMessage = errors.Count > 0 ? errors[0] : "Registration failed";
                ShowError(errorMessage);
            }
        }
        catch (Exception ex)
        {
            ShowError($"Error: {ex.Message}");
        }
    }

    private void ShowError(string message)
    {
        if (_registerErrorText != null)
        {
            _registerErrorText.Text = message;
            _registerErrorText.IsVisible = true;
        }
    }
}
