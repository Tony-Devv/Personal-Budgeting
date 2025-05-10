using System;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Controller;
using Model.Entities;
using System.Text.RegularExpressions;
using PersonalBudgeting.Views;

namespace PersonalBudgeting.Views.Pages;

public partial class SettingsPage : UserControl
{
    private readonly UserController _userController;
    private readonly User _currentUser;
    private readonly TextBox? _nameInput;
    private readonly TextBox? _emailInput;
    private readonly TextBlock? _errorText;
    private readonly TextBlock? _successText;
    
    // Control references
    private TextBlock? _userInitials;
    private TextBox? _userNameTextBox;
    private TextBox? _userEmailTextBox;
    private TextBox? _userPhoneTextBox;
    private ComboBox? _currencyComboBox;
    private ComboBox? _themeComboBox;
    private ComboBox? _accentColorComboBox;
    private CheckBox? _budgetAlertsCheckBox;
    private CheckBox? _paymentRemindersCheckBox;
    private CheckBox? _monthlySummaryCheckBox;

    public SettingsPage(UserController userController, User? currentUser)
    {
        InitializeComponent();
        
        _userController = userController;
        _currentUser = currentUser ?? new User { Id = 0, UserName = "Guest", Email = "guest@example.com" };
        
        // Get control references
        _nameInput = this.FindControl<TextBox>("NameInput");
        _emailInput = this.FindControl<TextBox>("EmailInput");
        _errorText = this.FindControl<TextBlock>("ErrorText");
        _successText = this.FindControl<TextBlock>("SuccessText");
        
        // Set initial values
        if (_nameInput != null)
            _nameInput.Text = _currentUser.UserName;
            
        if (_emailInput != null)
            _emailInput.Text = _currentUser.Email;
            
        // Hide messages
        if (_errorText != null)
            _errorText.IsVisible = false;
            
        if (_successText != null)
            _successText.IsVisible = false;
        
        LoadUserData();
        
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
        _userNameTextBox = this.FindControl<TextBox>("UserNameTextBox");
        _userEmailTextBox = this.FindControl<TextBox>("UserEmailTextBox");
        _userPhoneTextBox = this.FindControl<TextBox>("UserPhoneTextBox");
        _currencyComboBox = this.FindControl<ComboBox>("CurrencyComboBox");
        _themeComboBox = this.FindControl<ComboBox>("ThemeComboBox");
        _accentColorComboBox = this.FindControl<ComboBox>("AccentColorComboBox");
        _budgetAlertsCheckBox = this.FindControl<CheckBox>("BudgetAlertsCheckBox");
        _paymentRemindersCheckBox = this.FindControl<CheckBox>("PaymentRemindersCheckBox");
        _monthlySummaryCheckBox = this.FindControl<CheckBox>("MonthlySummaryCheckBox");
    }
    
    private void LoadUserData()
    {
        if (_currentUser == null)
            return;
            
        var nameTextBox = this.FindControl<TextBox>("NameTextBox");
        var emailTextBox = this.FindControl<TextBox>("EmailTextBox");
        var phoneTextBox = this.FindControl<TextBox>("PhoneTextBox");
        var currencyComboBox = this.FindControl<ComboBox>("CurrencyComboBox");
        var userInitials = this.FindControl<TextBlock>("UserInitials");
        
        if (nameTextBox != null)
            nameTextBox.Text = _currentUser.UserName;
            
        if (emailTextBox != null)
            emailTextBox.Text = _currentUser.Email;
            
        if (phoneTextBox != null)
            phoneTextBox.Text = _currentUser.PhoneNumber;
            
        if (currencyComboBox != null)
            currencyComboBox.SelectedIndex = 0; // Default to first currency
        
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
            // Validate inputs
            if (_nameInput == null || _emailInput == null)
                return;
                
            var name = _nameInput.Text;
            var email = _emailInput.Text;
            var phoneTextBox = this.FindControl<TextBox>("PhoneTextBox");
            var phone = phoneTextBox?.Text ?? _currentUser.PhoneNumber;
            
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
            {
                ShowError("Please enter your name and email");
                return;
            }

            // Create updated user object with current values
            var updatedUser = new User
            {
                Id = _currentUser.Id,
                UserName = name,
                Email = email,
                PhoneNumber = phone,
                Password = _currentUser.Password // Keep the current password
            };
            
            // Save user data to database
            var (success, user, errors) = await _userController.TryUpdateUser(updatedUser);
            
            if (success && user != null)
            {
                // Update current user reference with the updated values
                _currentUser.UserName = user.UserName;
                _currentUser.Email = user.Email;
                _currentUser.PhoneNumber = user.PhoneNumber;
                
                // Show success message
                ShowSuccess("Profile updated successfully");
                
                // Update UI with new values
                LoadUserData();
            }
            else
            {
                // Show error messages
                ShowError(errors.Count > 0 ? string.Join("\n", errors) : "Failed to update profile");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Error: {ex.Message}");
        }
    }
    
    private void SaveUserData(string name, string email)
    {
        // This method is now obsolete as we're using TryUpdateUser
        // Keeping it for backward compatibility
        _currentUser.UserName = name ?? string.Empty;
        _currentUser.Email = email ?? string.Empty;
    }
    
    private void ShowError(string message)
    {
        if (_errorText != null)
        {
            _errorText.Text = message;
            _errorText.IsVisible = true;
        }
        
        if (_successText != null)
        {
            _successText.IsVisible = false;
        }
    }
    
    private void ShowSuccess(string message)
    {
        if (_successText != null)
        {
            _successText.Text = message;
            _successText.IsVisible = true;
        }
        
        if (_errorText != null)
        {
            _errorText.IsVisible = false;
        }
    }
    
    private MainWindow? GetMainWindow()
    {
        var topLevel = TopLevel.GetTopLevel(this);
        return topLevel as MainWindow;
    }
    
    private string GetInitials(string name)
    {
        if (string.IsNullOrEmpty(name))
            return "U";
            
        var parts = name.Split(' ');
        if (parts.Length == 1)
            return name.Substring(0, Math.Min(2, name.Length)).ToUpper();
            
        return (parts[0][0].ToString() + parts[parts.Length - 1][0].ToString()).ToUpper();
    }

    private int GetCurrencyIndex(string currency)
    {
        return currency switch
        {
            "USD" => 0,
            "EUR" => 1,
            "GBP" => 2,
            "JPY" => 3,
            "CAD" => 4,
            _ => 0,
        };
    }

    private string GetCurrencyFromIndex(int index)
    {
        return index switch
        {
            0 => "USD",
            1 => "EUR",
            2 => "GBP",
            3 => "JPY",
            4 => "CAD",
            _ => "USD",
        };
    }

    private int GetThemeIndex(string theme)
    {
        return theme switch
        {
            "Dark" => 0,
            "Light" => 1,
            "System" => 2,
            _ => 0,
        };
    }

    private string GetThemeFromIndex(int index)
    {
        return index switch
        {
            0 => "Dark",
            1 => "Light",
            2 => "System",
            _ => "Dark",
        };
    }

    private int GetAccentColorIndex(string accentColor)
    {
        return accentColor switch
        {
            "Green" => 0,
            "Blue" => 1,
            "Purple" => 2,
            "Orange" => 3,
            _ => 0,
        };
    }

    private string GetAccentColorFromIndex(int index)
    {
        return index switch
        {
            0 => "Green",
            1 => "Blue",
            2 => "Purple",
            3 => "Orange",
            _ => "Green",
        };
    }

    private async void OnChangePasswordClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var currentPasswordBox = this.FindControl<TextBox>("CurrentPasswordBox");
            var newPasswordBox = this.FindControl<TextBox>("NewPasswordBox");
            var confirmPasswordBox = this.FindControl<TextBox>("ConfirmPasswordBox");
            
            if (currentPasswordBox == null || newPasswordBox == null || confirmPasswordBox == null)
            {
                ShowError("Password fields not found");
                return;
            }
            
            var currentPassword = currentPasswordBox.Text;
            var newPassword = newPasswordBox.Text;
            var confirmPassword = confirmPasswordBox.Text;
            
            // Validate inputs
            if (string.IsNullOrWhiteSpace(currentPassword) || 
                string.IsNullOrWhiteSpace(newPassword) || 
                string.IsNullOrWhiteSpace(confirmPassword))
            {
                ShowError("All password fields are required");
                return;
            }
            
            if (newPassword != confirmPassword)
            {
                ShowError("New password and confirmation do not match");
                return;
            }
            
            // Create user object with current email and password for verification
            var userWithCurrentPassword = new User
            {
                Id = _currentUser.Id,
                Email = _currentUser.Email,
                Password = currentPassword
            };
            
            // Change password using controller
            var (success, updatedUser, errors) = await _userController.TryChangeUserPassword(newPassword, userWithCurrentPassword);
            
            if (success && updatedUser != null)
            {
                // Update current user reference
                _currentUser.Password = updatedUser.Password;
                
                // Clear password fields
                currentPasswordBox.Text = string.Empty;
                newPasswordBox.Text = string.Empty;
                confirmPasswordBox.Text = string.Empty;
                
                // Show success message
                ShowSuccess("Password changed successfully");
            }
            else
            {
                // Show error messages
                ShowError(errors.Count > 0 ? string.Join("\n", errors) : "Failed to change password");
            }
        }
        catch (Exception ex)
        {
            ShowError($"Error: {ex.Message}");
        }
    }
}
