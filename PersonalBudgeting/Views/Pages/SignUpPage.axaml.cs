using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Controller;
using Model.Entities;

namespace PersonalBudgeting.Views.Pages;

public partial class SignUpPage : UserControl
{
    private readonly UserController _userController;
    private readonly ContentControl _contentFrame;
    private readonly Action<int> _onLoginSuccess;
    private readonly TextBlock? _errorText;

    public SignUpPage(UserController userController, ContentControl contentFrame, Action<int> onLoginSuccess)
    {
        InitializeComponent();
        _userController = userController;
        _contentFrame = contentFrame;
        _onLoginSuccess = onLoginSuccess;
        _errorText = this.FindControl<TextBlock>("ErrorText");
    }

    private async void OnSignUpClick(object? sender, RoutedEventArgs e)
    {
        var usernameInput = this.FindControl<TextBox>("UsernameInput") ?? 
            throw new InvalidOperationException("UsernameInput not found");
        var emailInput = this.FindControl<TextBox>("EmailInput") ?? 
            throw new InvalidOperationException("EmailInput not found");
        var phoneInput = this.FindControl<TextBox>("PhoneInput") ?? 
            throw new InvalidOperationException("PhoneInput not found");
        var passwordInput = this.FindControl<TextBox>("PasswordInput") ?? 
            throw new InvalidOperationException("PasswordInput not found");
        var confirmPasswordInput = this.FindControl<TextBox>("ConfirmPasswordInput") ?? 
            throw new InvalidOperationException("ConfirmPasswordInput not found");

        // Basic validation
        if (string.IsNullOrWhiteSpace(usernameInput.Text) ||
            string.IsNullOrWhiteSpace(emailInput.Text) ||
            string.IsNullOrWhiteSpace(passwordInput.Text))
        {
            ShowError("Username, email, and password are required");
            return;
        }

        if (passwordInput.Text != confirmPasswordInput.Text)
        {
            ShowError("Passwords do not match");
            return;
        }

        // Create user model with registration data
        var user = new User
        {
            UserName = usernameInput.Text.Trim(),
            Email = emailInput.Text.Trim(),
            PhoneNumber = phoneInput.Text?.Trim() ?? string.Empty,
            Password = passwordInput.Text.Trim()
        };

        try
        {
            // Use controller to register the user in the database
            var (success, newUser, errors) = await _userController.TryAddUser(user);
            
            if (success && newUser != null)
            {
                // Registration successful, proceed to main window
                Console.WriteLine($"User registered successfully: {newUser.UserName} (ID: {newUser.Id})");
                _onLoginSuccess(newUser.Id);
            }
            else
            {
                // Registration failed
                ShowError(string.Join("\n", errors));
                Console.WriteLine("Registration failed: " + string.Join(", ", errors));
            }
        }
        catch (Exception ex)
        {
            ShowError($"An error occurred: {ex.Message}");
            Console.WriteLine($"Error during registration: {ex.Message}");
        }
    }

    private void OnBackClick(object? sender, RoutedEventArgs e)
    {
        _contentFrame.Content = new WelcomePage(_userController, _contentFrame, _onLoginSuccess);
    }

    private void OnLoginClick(object? sender, RoutedEventArgs e)
    {
        _contentFrame.Content = new LoginPage(_userController, _contentFrame, _onLoginSuccess);
    }

    private void ShowError(string message)
    {
        if (_errorText != null)
        {
            _errorText.Text = message;
            _errorText.IsVisible = true;
        }
    }
}
