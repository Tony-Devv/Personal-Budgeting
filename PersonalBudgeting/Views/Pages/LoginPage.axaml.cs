using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.VisualTree;
using Controller;
using Model.Entities;
using PersonalBudgeting.Views.Pages;
using PersonalBudgeting.Controllers;
using PersonalBudgeting.Models;
using User = PersonalBudgeting.Models.User;

namespace PersonalBudgeting.Views.Pages;

public partial class LoginPage : UserControl
{
    private readonly UserController _userController;
    private readonly ContentControl? _contentFrame;
    private readonly Action<int>? _onLoginSuccess;
    private readonly TextBlock? _errorText;
    
    // UI elements
    private TextBox? _usernameInput;
    private TextBox? _passwordInput;
    private TextBlock? _loginErrorText;

    public LoginPage()
    {
        InitializeComponent();
        _userController = new UserController();
        _contentFrame = null;
        _onLoginSuccess = null;
        _errorText = null;
        GetControlReferences();
    }

    public LoginPage(UserController userController, ContentControl contentFrame, Action<int> onLoginSuccess)
    {
        InitializeComponent();
        _userController = userController;
        _contentFrame = contentFrame;
        _onLoginSuccess = onLoginSuccess;
        _errorText = this.FindControl<TextBlock>("LoginErrorText");
        GetControlReferences();
    }
    
    private void GetControlReferences()
    {
        _usernameInput = this.FindControl<TextBox>("UsernameInput");
        _passwordInput = this.FindControl<TextBox>("PasswordInput");
        _loginErrorText = this.FindControl<TextBlock>("LoginErrorText");
    }

    private void InitializeComponent()
    {
        Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
    }

    private async void OnLoginClick(object? sender, RoutedEventArgs e)
    {
        // Get input values
        var usernameOrEmail = _usernameInput?.Text?.Trim() ?? string.Empty;
        var password = _passwordInput?.Text?.Trim() ?? string.Empty;
        
        // Validate input
        if (string.IsNullOrWhiteSpace(usernameOrEmail) || string.IsNullOrWhiteSpace(password))
        {
            if (_loginErrorText != null)
            {
                _loginErrorText.Text = "Email and password are required";
                _loginErrorText.IsVisible = true;
            }
            return;
        }
        
        try
        {
            // Create login user
            var loginUser = new Model.Entities.User { 
                Email = usernameOrEmail,
                Password = password
            };
            
            // Attempt login
            if (_userController != null)
            {
                var result = await _userController.TryLoginUser(loginUser);
                
                if (result.Success && result.User != null)
                {
                    // Login successful
                    // Use a null-safe callback to prevent null reference exceptions
                    var safeLoginSuccess = _onLoginSuccess ?? (id => {});
                    
                    if (_contentFrame != null)
                    {
                        _contentFrame.Content = null; // Clear content before callback
                        safeLoginSuccess(result.User.Id);
                    }
                }
                else
                {
                    // Login failed
                    if (_loginErrorText != null)
                    {
                        _loginErrorText.Text = "Invalid email or password";
                        _loginErrorText.IsVisible = true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            if (_loginErrorText != null)
            {
                _loginErrorText.Text = $"Login error: {ex.Message}";
                _loginErrorText.IsVisible = true;
            }
            
        }
    }

    private void OnRegisterLinkClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Show registration form
            var loginForm = this.FindControl<Grid>("LoginForm");
            var registerForm = this.FindControl<Grid>("RegisterForm");
            
            if (loginForm != null && registerForm != null)
            {
                loginForm.IsVisible = false;
                registerForm.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
        }
    }

    private void OnLoginLinkClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Show login form
            var loginForm = this.FindControl<Grid>("LoginForm");
            var registerForm = this.FindControl<Grid>("RegisterForm");
            
            if (loginForm != null && registerForm != null)
            {
                loginForm.IsVisible = true;
                registerForm.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
        }
    }

    private async void OnRegisterClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Get registration inputs
            var nameInput = this.FindControl<TextBox>("NameInput");
            var emailInput = this.FindControl<TextBox>("EmailInput");
            var phoneInput = this.FindControl<TextBox>("PhoneInput");
            var regPasswordInput = this.FindControl<TextBox>("RegPasswordInput");
            var errorText = this.FindControl<TextBlock>("RegisterErrorText");
            
            if (nameInput == null || emailInput == null || 
                regPasswordInput == null || errorText == null)
                return;

            var name = nameInput.Text?.Trim();
            var email = emailInput.Text?.Trim();
            var phone = phoneInput?.Text?.Trim();
            var password = regPasswordInput.Text;
            
            // Validate input
            if (string.IsNullOrWhiteSpace(name))
            {
                errorText.Text = "Name is required";
                errorText.IsVisible = true;
                return;
            }
            
            if (string.IsNullOrWhiteSpace(email))
            {
                errorText.Text = "Email is required";
                errorText.IsVisible = true;
                return;
            }
            
            if (string.IsNullOrWhiteSpace(password))
            {
                errorText.Text = "Password is required";
                errorText.IsVisible = true;
                return;
            }
            
            if (password.Length < 6)
            {
                errorText.Text = "Password must be at least 6 characters";
                errorText.IsVisible = true;
                return;
            }
            
            // Create user model with registration data
            var newUser = new Model.Entities.User
            {
                UserName = name,
                Email = email,
                PhoneNumber = phone,
                Password = password
            };
            
            // Use the controller to register the user
            var result = await _userController.TryAddUser(newUser);
            
            if (result.Success && result.User != null)
            {
                // Registration successful, proceed to login
                
                // Call onLoginSuccess to navigate to main window
                if (_onLoginSuccess != null)
                {
                    _onLoginSuccess(result.User.Id);
                }
            }
            else
            {
                // Registration failed
                errorText.Text = result.errorMessages.Count > 0 ? string.Join(", ", result.errorMessages) : "Failed to register user";
                errorText.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            var errorText = this.FindControl<TextBlock>("RegisterErrorText");
            if (errorText != null)
            {
                errorText.Text = "An error occurred during registration";
                errorText.IsVisible = true;
            }
        }
    }

    private void NavigateToMainWindow(Model.Entities.User user)
    {
        // Call the login success callback instead of creating a new window
        if (_onLoginSuccess != null && _contentFrame != null)
        {
            _onLoginSuccess(user.Id);
        }
    }

    private void ShowError(string message)
    {
        if (_errorText != null)
        {
            _errorText.Text = message;
            _errorText.IsVisible = true;
        }
    }

    private void OnBackClick(object? sender, RoutedEventArgs e)
    {
        // Check if _contentFrame and _userController are not null before using them
        if (_contentFrame == null || _userController == null) 
        {
            // Log the issue and return early to avoid null reference errors
            return;
        }
        
        // Use a null-safe callback for _onLoginSuccess
        var safeLoginSuccess = _onLoginSuccess ?? (id => { });
        
        // Navigate to the welcome page with all required parameters
        try
        {
            _contentFrame.Content = new WelcomePage(_userController!, _contentFrame!, safeLoginSuccess);
        }
        catch (Exception ex)
        {
        }
    }
}
