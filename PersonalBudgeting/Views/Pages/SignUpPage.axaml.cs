using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Controller;
using Model.Entities;
using PersonalBudgeting.Views;
using Avalonia.Controls.Primitives;
using Avalonia.Media;

namespace PersonalBudgeting.Views.Pages;

public partial class SignUpPage : UserControl
{
    private readonly UserController _userController;
    private readonly ContentControl _contentFrame;
    private readonly Action<int> _onLoginSuccess;

    // UI elements
    private TextBox? _usernameInput;
    private TextBox? _emailInput;
    private TextBox? _passwordInput;
    private TextBox? _phoneInput;
    private TextBlock? _registerErrorText;

    public SignUpPage(UserController userController, ContentControl contentFrame, Action<int> onLoginSuccess)
    {
        InitializeComponent();
        _userController = userController ?? throw new ArgumentNullException(nameof(userController));
        _contentFrame = contentFrame ?? throw new ArgumentNullException(nameof(contentFrame));
        _onLoginSuccess = onLoginSuccess ?? throw new ArgumentNullException(nameof(onLoginSuccess));

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
        // Check if control references are initialized
        if (_usernameInput == null || _emailInput == null || _passwordInput == null || 
            _phoneInput == null || _registerErrorText == null || _userController == null)
        {
            Console.WriteLine("Error: UI controls are not properly initialized");
            return;
        }
        
        try
        {
            // Get input values
            var username = _usernameInput.Text?.Trim() ?? string.Empty;
            var email = _emailInput.Text?.Trim() ?? string.Empty;
            var password = _passwordInput.Text ?? string.Empty;
            var phone = _phoneInput.Text?.Trim() ?? string.Empty;
            
            // Validate input
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(email) || 
                string.IsNullOrWhiteSpace(password))
            {
                _registerErrorText.Text = "Username, email, and password are required";
                _registerErrorText.IsVisible = true;
                return;
            }
            
            // Create user object
            var newUser = new Model.Entities.User
            {
                UserName = username,
                Email = email,
                Password = password,
                PhoneNumber = phone
            };
            
            // Attempt to register user
            var result = await _userController.TryAddUser(newUser);
            
            if (result.Success && result.User != null)
            {
                // Registration successful - show success message then navigate to login page
                _registerErrorText.Text = "Registration successful! Redirecting to login...";
                _registerErrorText.Foreground = new SolidColorBrush(Colors.Green);
                _registerErrorText.IsVisible = true;
                
                // Add delay for user to see success message
                await Task.Delay(1500);
                
                // Use a null-safe callback to prevent null reference exceptions
                var safeLoginSuccess = _onLoginSuccess ?? (id => { });
                
                if (_contentFrame != null)
                {
                    // Navigate to login
                    _contentFrame.Content = new LoginPage(_userController, _contentFrame, safeLoginSuccess);
                }
            }
            else
            {
                // Registration failed - show error
                var errorMessage = result.errorMessages.Count > 0 ? string.Join(", ", result.errorMessages) : "Registration failed";
                _registerErrorText.Text = errorMessage;
                _registerErrorText.Foreground = new SolidColorBrush(Colors.Red);
                _registerErrorText.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            // Log and show error
            Console.WriteLine($"Error during registration: {ex.Message}");
            _registerErrorText.Text = "An error occurred during registration";
            _registerErrorText.Foreground = new SolidColorBrush(Colors.Red);
            _registerErrorText.IsVisible = true;
        }
    }
}
