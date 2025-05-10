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

    public LoginPage()
    {
        InitializeComponent();
        _userController = new UserController();
        _contentFrame = null;
        _onLoginSuccess = null;
        _errorText = null;
    }

    public LoginPage(UserController userController, ContentControl contentFrame, Action<int> onLoginSuccess)
    {
        InitializeComponent();
        _userController = userController;
        _contentFrame = contentFrame;
        _onLoginSuccess = onLoginSuccess;
        _errorText = this.FindControl<TextBlock>("LoginErrorText");
    }

    private void InitializeComponent()
    {
        Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
    }

    private async void OnLoginClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Get login inputs
            var usernameInput = this.FindControl<TextBox>("UsernameInput");
            var passwordInput = this.FindControl<TextBox>("PasswordInput");
            var errorText = this.FindControl<TextBlock>("LoginErrorText");
            
            if (usernameInput == null || passwordInput == null || errorText == null)
                return;
            
            var usernameOrEmail = usernameInput.Text?.Trim();
            var password = passwordInput.Text;
            
            // Validate input
            if (string.IsNullOrWhiteSpace(usernameOrEmail) || string.IsNullOrWhiteSpace(password))
            {
                errorText.Text = "Email and password are required";
                errorText.IsVisible = true;
                return;
            }
            
            // Authenticate using the UserController
            // Note: The validator requires Email, not UserName for login
            var loginUser = new Model.Entities.User { 
                Email = usernameOrEmail, // Use the input as Email since that's what the validator checks
                Password = password
            };
            
            // Attempt login with database
            var (success, user, errors) = await _userController.TryLoginUser(loginUser);
            
            if (success && user != null)
            {
                // Database login successful
                Console.WriteLine($"Login successful for user: {user.UserName} (ID: {user.Id})");
                
                // Call the onLoginSuccess callback
                if (_onLoginSuccess != null)
                {
                    _onLoginSuccess(user.Id);
                }
            }
            else 
            {
                // Login failed - show error
                errorText.Text = errors.Count > 0 ? string.Join(", ", errors) : "Invalid email or password";
                errorText.IsVisible = true;
                Console.WriteLine("Login failed: " + (errors.Count > 0 ? string.Join(", ", errors) : "Unknown error"));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during login: {ex.Message}");
            var errorText = this.FindControl<TextBlock>("LoginErrorText");
            if (errorText != null)
            {
                errorText.Text = "An error occurred during login";
                errorText.IsVisible = true;
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
            Console.WriteLine($"Error navigating to register form: {ex.Message}");
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
            Console.WriteLine($"Error navigating to login form: {ex.Message}");
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
            var (success, registeredUser, errors) = await _userController.TryAddUser(newUser);
            
            if (success && registeredUser != null)
            {
                // Registration successful, proceed to login
                Console.WriteLine($"User registered successfully: {registeredUser.UserName} (ID: {registeredUser.Id})");
                
                // Call onLoginSuccess to navigate to main window
                if (_onLoginSuccess != null)
                {
                    _onLoginSuccess(registeredUser.Id);
                }
            }
            else
            {
                // Registration failed
                errorText.Text = errors.Count > 0 ? string.Join(", ", errors) : "Failed to register user";
                errorText.IsVisible = true;
                Console.WriteLine("Registration failed: " + (errors.Count > 0 ? string.Join(", ", errors) : "Unknown error"));
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during registration: {ex.Message}");
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
        if (_contentFrame != null)
        {
            _contentFrame.Content = new WelcomePage(_userController, _contentFrame, _onLoginSuccess ?? (id => { }));
        }
    }
}
