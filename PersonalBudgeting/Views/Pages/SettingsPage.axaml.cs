using System;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Controller;
using Model.Entities;
using System.Text.RegularExpressions;
using PersonalBudgeting.Views;
using Avalonia.Media;
using Avalonia.Platform.Storage;
using System.Linq;
using static Avalonia.Application;
using System.Reflection;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Styling;
using System.Collections.Generic;

namespace PersonalBudgeting.Views.Pages;

public partial class SettingsPage : UserControl
{
    private readonly UserController _userController;
    private readonly User _currentUser;
    
    // Control references
    private TextBlock? _userInitials;
    private TextBox? _userNameTextBox;
    private TextBox? _userEmailTextBox;
    private TextBox? _userPhoneTextBox;
    private TextBlock? _profileErrorText;
    private TextBlock? _passwordErrorText;
    private TextBox? _oldPasswordTextBox;
    private TextBox? _newPasswordTextBox;

    public SettingsPage(UserController userController, User? currentUser)
    {
        InitializeComponent();
        
        _userController = userController;
        _currentUser = currentUser ?? new User { Id = 0, UserName = "Guest", Email = "guest@example.com" };
        
        LoadUserData();
        GetControlReferences();
        
        // Attach event handlers
        var logoutButton = this.FindControl<Button>("LogoutButton");
        if (logoutButton != null)
        {
            logoutButton.Click += OnLogoutClick;
        }
        
        var saveButton = this.FindControl<Button>("SaveSettingsButton");
        if (saveButton != null)
        {
            saveButton.Click += OnSaveClick;
        }
        
        var changePasswordButton = this.FindControl<Button>("ChangePasswordButton");
        if (changePasswordButton != null)
        {
            changePasswordButton.Click += OnChangePasswordClick;
        }
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    
    private void GetControlReferences()
    {
        _userInitials = this.FindControl<TextBlock>("UserInitials");
        _userNameTextBox = this.FindControl<TextBox>("NameTextBox");
        _userEmailTextBox = this.FindControl<TextBox>("EmailTextBox");
        _userPhoneTextBox = this.FindControl<TextBox>("PhoneTextBox");
        _profileErrorText = this.FindControl<TextBlock>("ProfileErrorText");
        _passwordErrorText = this.FindControl<TextBlock>("PasswordErrorText");
        _oldPasswordTextBox = this.FindControl<TextBox>("OldPasswordTextBox");
        _newPasswordTextBox = this.FindControl<TextBox>("NewPasswordTextBox");
    }
    
    private void LoadUserData()
    {
        if (_currentUser == null)
            return;
            
        var nameTextBox = this.FindControl<TextBox>("NameTextBox");
        var emailTextBox = this.FindControl<TextBox>("EmailTextBox");
        var phoneTextBox = this.FindControl<TextBox>("PhoneTextBox");
        var userInitials = this.FindControl<TextBlock>("UserInitials");
        
        if (nameTextBox != null)
            nameTextBox.Text = _currentUser.UserName;
            
        if (emailTextBox != null)
            emailTextBox.Text = _currentUser.Email;
            
        if (phoneTextBox != null)
            phoneTextBox.Text = _currentUser.PhoneNumber;
        
        if (userInitials != null && !string.IsNullOrEmpty(_currentUser.UserName))
        {
            userInitials.Text = GetInitials(_currentUser.UserName);
        }
    }
    
    private void OnLogoutClick(object? sender, RoutedEventArgs e)
    {
        // Navigate to welcome page using MainWindow navigation
        var mainWindow = TopLevel.GetTopLevel(this) as MainWindow;
        if (mainWindow != null)
        {
            // Create new event arguments if sender is null
            var eventArgs = e ?? new RoutedEventArgs();
            mainWindow.OnLogoutClick(sender ?? this, eventArgs);
        }
    }
    
    private async void OnSaveClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Ensure we have the control references
            GetControlReferences();
            
            // Validate inputs
            if (_userNameTextBox == null || _userEmailTextBox == null || _userPhoneTextBox == null || _profileErrorText == null)
            {
                ShowError("One or more input controls is null", _profileErrorText);
                return;
            }
            
            var name = _userNameTextBox.Text?.Trim() ?? string.Empty;
            var email = _userEmailTextBox.Text?.Trim() ?? string.Empty;
            var phone = _userPhoneTextBox.Text?.Trim() ?? string.Empty;
            
            // Validate required fields
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
            {
                ShowError("Please enter your name and email", _profileErrorText);
                return;
            }
            
            // Validate username length (max 100 chars)
            if (name.Length > 100)
            {
                ShowError("Username cannot exceed 100 characters", _profileErrorText);
                return;
            }
            
            // Validate email format
            if (!IsValidEmail(email))
            {
                ShowError("Please enter a valid email address", _profileErrorText);
                return;
            }
            
            // Validate email length (max 100 chars)
            if (email.Length > 100)
            {
                ShowError("Email cannot exceed 100 characters", _profileErrorText);
                return;
            }
            
            // Validate phone number length (max 15 chars)
            if (phone.Length > 15)
            {
                ShowError("Phone number cannot exceed 15 characters", _profileErrorText);
                return;
            }

            // Create updated user object with current values (keep password unchanged)
            var updatedUser = new User
            {
                Id = _currentUser.Id,
                UserName = name,
                Email = email,
                PhoneNumber = phone,
                Password = _currentUser.Password // Keep the current password
            };
            
            // Save user data to database
            var result = await _userController.TryUpdateUser(updatedUser);
            
            if (result.Success && result.UpdatedUser != null)
            {
                // Update current user reference with the updated values
                _currentUser.UserName = result.UpdatedUser.UserName;
                _currentUser.Email = result.UpdatedUser.Email;
                _currentUser.PhoneNumber = result.UpdatedUser.PhoneNumber;
                
                // Show success message
                ShowSuccess("Profile updated successfully", _profileErrorText);
                
                // Update UI with new values (including initials)
                if (_userInitials != null && !string.IsNullOrEmpty(_currentUser.UserName))
                {
                    _userInitials.Text = GetInitials(_currentUser.UserName);
                }
            }
            else
            {
                // Show error messages
                var errorMessage = result.errors != null && result.errors.Count > 0 
                    ? string.Join("\n", result.errors) 
                    : "Failed to update profile";
                ShowError(errorMessage, _profileErrorText);
            }
        }
        catch (Exception ex)
        {
            ShowError($"An error occurred: {ex.Message}", _profileErrorText);
        }
    }
    
    private async void OnChangePasswordClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Ensure we have the control references
            GetControlReferences();
            
            if (_oldPasswordTextBox == null || _newPasswordTextBox == null || _passwordErrorText == null)
            {
                ShowError("One or more password controls is null", _passwordErrorText);
                return;
            }
            
            var oldPassword = _oldPasswordTextBox.Text?.Trim() ?? string.Empty;
            var newPassword = _newPasswordTextBox.Text?.Trim() ?? string.Empty;
            
            // Validate fields
            if (string.IsNullOrWhiteSpace(oldPassword) || string.IsNullOrWhiteSpace(newPassword))
            {
                ShowError("Please enter both old and new password", _passwordErrorText);
                return;
            }
            
            // Validate password length
            if (newPassword.Length < 6)
            {
                ShowError("New password must be at least 6 characters", _passwordErrorText);
                return;
            }
            
            // Create user object with ID, Email, and old password for verification
            var userWithOldPassword = new User
            {
                Id = _currentUser.Id,
                Email = _currentUser.Email, 
                UserName = _currentUser.UserName,
                Password = oldPassword       
            };
            
            
            // Call TryChangeUserPassword with newPassword and user with old password
            var result = await _userController.TryChangeUserPassword(newPassword, userWithOldPassword);
            
            if (result.Success && result.UpdatedUser != null)
            {
                // Show success message
                ShowSuccess("Password changed successfully", _passwordErrorText);
                
                // Clear password fields
                _oldPasswordTextBox.Text = string.Empty;
                _newPasswordTextBox.Text = string.Empty;
            }
            else
            {
                // Show error messages (likely wrong old password)
                var errorMessage = result.errors != null && result.errors.Count > 0 
                    ? string.Join("\n", result.errors) 
                    : "Failed to change password. Please check your current password.";
                ShowError(errorMessage, _passwordErrorText);
            }
        }
        catch (Exception ex)
        {
            ShowError($"An error occurred: {ex.Message}", _passwordErrorText);
        }
    }
    
    private bool IsValidEmail(string email)
    {
        try
        {
            // Simple email validation using regex
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
        catch
        {
            return false;
        }
    }
    
    // Helper method to show error with specific TextBlock
    private void ShowError(string message, TextBlock? errorText = null)
    {
        if (errorText == null)
        {
            // Try to find appropriate error text block based on context
            if (_profileErrorText != null)
            {
                errorText = _profileErrorText;
            }
            else if (_passwordErrorText != null)
            {
                errorText = _passwordErrorText;
            }
            else
            {
                // If still null, try to find it directly
                errorText = this.FindControl<TextBlock>("ProfileErrorText") ?? 
                            this.FindControl<TextBlock>("PasswordErrorText");
            }
        }
        
        if (errorText != null)
        {
            errorText.Text = message;
            errorText.IsVisible = true;
            errorText.Foreground = new SolidColorBrush(Colors.Red);
        }
    }
    
    // Helper method to show success with specific TextBlock
    private void ShowSuccess(string message, TextBlock? errorText = null)
    {
        if (errorText == null)
        {
            // Try to find appropriate error text block based on context
            if (_profileErrorText != null)
            {
                errorText = _profileErrorText;
            }
            else if (_passwordErrorText != null)
            {
                errorText = _passwordErrorText;
            }
            else
            {
                // If still null, try to find it directly
                errorText = this.FindControl<TextBlock>("ProfileErrorText") ?? 
                            this.FindControl<TextBlock>("PasswordErrorText");
            }
        }
        
        if (errorText != null)
        {
            errorText.Text = message;
            errorText.IsVisible = true;
            errorText.Foreground = new SolidColorBrush(Colors.Green);
            
            // Hide after 3 seconds
            Task.Delay(3000).ContinueWith(_ => 
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(() =>
                {
                    errorText.IsVisible = false;
                });
            });
        }
    }
    
    private string GetInitials(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return "??";
            
        var parts = name.Split(' ');
        if (parts.Length >= 2)
        {
            return (parts[0].FirstOrDefault().ToString() + parts[1].FirstOrDefault().ToString()).ToUpper();
        }
        return name.Length >= 2 ? name.Substring(0, 2).ToUpper() : name + "?";
    }
}
