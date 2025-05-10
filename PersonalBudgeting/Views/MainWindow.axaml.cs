using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Interactivity;
using Avalonia.Media;
using Controller;
using Model.Entities;
using PersonalBudgeting.Views.Pages;
using Avalonia.VisualTree;

namespace PersonalBudgeting.Views;

public partial class MainWindow : Window
{
    // Controllers
    private readonly UserController _userController;
    private readonly IncomeController _incomeController;
    private readonly ExpenseController _expenseController;
    private readonly BudgetController _budgetController;
    
    // Fields
    private User? _currentUser;
    private int _currentUserId = 0;
    private readonly ContentControl _pageContent;
    private readonly ScrollViewer _sidebarMenu;
    private readonly TextBlock? _userInitials;
    private readonly TextBlock? _userNameText;
    private string _currentEditField = string.Empty;

    public MainWindow()
    {
        InitializeComponent();
        
        // Initialize controllers
        _incomeController = new IncomeController();
        _expenseController = new ExpenseController();
        _budgetController = new BudgetController();
        _userController = new UserController();
        
        // Remove the test user initialization - we'll get real users from the database
        _currentUser = null;
        
        // Find controls
        _pageContent = this.FindControl<ContentControl>("PageContent") ?? 
            throw new InvalidOperationException("PageContent not found");
        _sidebarMenu = this.FindControl<ScrollViewer>("SidebarMenu") ?? 
            throw new InvalidOperationException("SidebarMenu not found");
        _userInitials = this.FindControl<TextBlock>("UserInitials");
        _userNameText = this.FindControl<TextBlock>("UsernameText");
        
        // Set username to indicate login is required
        if (_userNameText != null)
        {
            _userNameText.Text = "Login First";
        }
        
        // Set user initials
        if (_userInitials != null)
        {
            _userInitials.Text = "?";
        }
        
        // Start with welcome page instead of home page
        ShowWelcomePage();
    }
    
    // Method to handle successful login
    private async void OnLoginSuccess(int userId)
    {
        try {
            // Set the current user ID
            _currentUserId = userId;
            
            // Create a temporary user with only the ID
            _currentUser = new User { Id = _currentUserId };
            
            // Make sure sidebar is visible
            _sidebarMenu.IsVisible = true;
            
            // Fetch the user's detailed info
            var (success, userDetails, errors) = await _userController.TryGetUserById(_currentUserId);
            
            if (success && userDetails != null)
            {
                // Update the current user with the detailed information
                _currentUser.UserName = userDetails.UserName ?? "User";
                _currentUser.Email = userDetails.Email;
                _currentUser.PhoneNumber = userDetails.PhoneNumber;
                
                Console.WriteLine($"Logged in as user: {_currentUser.UserName} (ID: {_currentUserId})");
            }
            else
            {
                Console.WriteLine($"Logged in with minimal user info (ID: {_currentUserId})");
            }
            
            // Always proceed to load the home page for valid users
            LoadHomePage();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in OnLoginSuccess: {ex.Message}");
            ShowWelcomePage();
        }
    }
    
    // Method to load user data from the database
    private async Task LoadUserData()
    {
        try
        {
            // Get the user by ID
            var user = new User { Id = _currentUserId };
            var (incomesSuccess, userIncomes, incomeErrors) = await _userController.TryGetUserIncomes(user);
            
            if (incomesSuccess && userIncomes != null && userIncomes.Count > 0)
            {
                // We have user data from the database
                _currentUser = user;
                
                // Update UI with user details
                if (_userInitials != null)
                    _userInitials.Text = GetInitials(_currentUser.UserName);
                if (_userNameText != null)
                    _userNameText.Text = _currentUser.UserName;
                
                // Show the navigation menu
                _sidebarMenu.IsVisible = true;
            }
            else
            {
                // No user found, show welcome page
                ShowWelcomePage();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading user data: {ex.Message}");
            ShowWelcomePage();
        }
    }
    
    // Helper to get user initials from name
    private string GetInitials(string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return "?";
            
        var parts = fullName.Split(' ');
        if (parts.Length == 1)
            return parts[0].Length > 0 ? parts[0][0].ToString().ToUpper() : "?";
            
        return $"{parts[0][0]}{parts[^1][0]}".ToUpper();
    }
    
    // Method to show the welcome page
    private void ShowWelcomePage()
    {
        _sidebarMenu.IsVisible = false;
        _pageContent.Content = new WelcomePage(_userController, _pageContent, OnLoginSuccess);
    }
    
    // Method to load the home page
    private void LoadHomePage()
    {
        try
        {
            _pageContent.Content = new HomePage(
                _userController,
                _currentUser,
                _pageContent
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading HomePage: {ex.Message}");
        }
    }
    
    // Method to navigate between pages
    public async void NavigateToPage(string? pageName)
    {
        // Check if user is loaded
        if (_currentUser == null && pageName != "welcome")
        {
            ShowWelcomePage();
            return;
        }
        
        try
        {
            // Use the current user
            if (_currentUser == null)
            {
                ShowWelcomePage();
                return;
            }
            
            // Navigate based on page name
            switch (pageName?.ToLower())
            {
                case "home":
                    _pageContent.Content = new HomePage(
                        _userController,
                        _currentUser,
                        _pageContent
                    );
                    break;
                    
                case "income":
                    var (incomesSuccess, userIncomes, incomeErrors) = await _userController.TryGetUserIncomes(_currentUser);
                    _pageContent.Content = new IncomePage(_incomeController, _userController, _currentUser, 
                        incomesSuccess ? userIncomes : new List<Income>());
                    break;
                    
                case "expenses":
                    var (expensesSuccess, userExpenses, expenseErrors) = await _userController.TryGetUserExpenses(_currentUser);
                    _pageContent.Content = new ExpensesPage(_expenseController, _userController, _currentUser);
                    break;
                    
                case "budget":
                    var (budgetsSuccess, userBudgets, budgetErrors) = await _userController.TryGetUserBudgets(_currentUser);
                    var (expSuccess, expensesForBudget, expErrors) = await _userController.TryGetUserExpenses(_currentUser);
                    
                    try
                    {
                        Console.WriteLine("Creating BudgetPage...");
                        var budgetPage = new BudgetPage(
                            _budgetController,
                            _userController,
                            _currentUser);
                        
                        Console.WriteLine("BudgetPage created successfully, setting as content");
                        _pageContent.Content = budgetPage;
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error creating BudgetPage: {ex.Message}");
                        Console.WriteLine($"Stack trace: {ex.StackTrace}");
                        
                        // Fallback to home page if budget page fails to load
                        LoadHomePage();
                    }
                    break;
                    
                case "settings":
                    _pageContent.Content = new SettingsPage(_userController, _currentUser);
                    break;
                    
                case "welcome":
                default:
                    ShowWelcomePage();
                    break;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error navigating to {pageName}: {ex.Message}");
        }
    }
    
    // Event handler for navigation button clicks
    private void OnNavButtonClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button)
        {
            // Navigate to the selected page
            switch (button.Name)
            {
                case "HomeButton":
                    NavigateToPage("home");
                    break;
                    
                case "IncomeButton":
                    NavigateToPage("income");
                    break;
                    
                case "ExpensesButton":
                    NavigateToPage("expenses");
                    break;
                    
                case "BudgetButton":
                    NavigateToPage("budget");
                    break;
                    
                case "SettingsButton":
                    NavigateToPage("settings");
                    break;
            }
        }
    }
    
    // Event handler for profile button click
    private void OnProfileClick(object sender, RoutedEventArgs e)
    {
        NavigateToPage("settings");
    }
    
    // Event handler for logout button click  
    public void OnLogoutClick(object sender, RoutedEventArgs e)
    {
        _currentUser = null;
        _currentUserId = 0;
        ShowWelcomePage();
    }
    
    // Add method to update user profile display
    private void UpdateUserProfileDisplay()
    {
        if (_currentUser == null) return;

        // Note: We removed the user profile card as requested,
        // so we only need to update the title on pages
        
        var currentPageTitle = this.FindControl<TextBlock>("CurrentPageTitle");
        if (currentPageTitle != null)
        {
            // Update the page title if needed
            // This keeps the title updated based on navigation
        }
    }
    
    // User edit event handlers
    private void OnEditUserNameClick(object? sender, RoutedEventArgs e)
    {
        _currentEditField = "UserName";
        ShowUserEditPopup("Edit Username", _currentUser?.UserName ?? string.Empty);
    }
    
    private void OnEditUserEmailClick(object? sender, RoutedEventArgs e)
    {
        _currentEditField = "Email";
        ShowUserEditPopup("Edit Email", _currentUser?.Email ?? string.Empty);
    }
    
    private void OnEditUserPhoneClick(object? sender, RoutedEventArgs e)
    {
        _currentEditField = "Phone";
        ShowUserEditPopup("Edit Phone Number", "Not set");
    }
    
    private void ShowUserEditPopup(string title, string currentValue)
    {
        var popup = this.FindControl<Border>("UserEditPopup");
        var titleText = this.FindControl<TextBlock>("UserEditPopupTitle");
        var textBox = this.FindControl<TextBox>("UserEditTextBox");
        var errorText = this.FindControl<TextBlock>("UserEditErrorText");
        
        if (popup == null || titleText == null || textBox == null || errorText == null)
            return;
        
        titleText.Text = title;
        textBox.Text = currentValue;
        errorText.IsVisible = false;
        
        popup.IsVisible = true;
    }
    
    private void OnCancelUserEditClick(object? sender, RoutedEventArgs e)
    {
        var popup = this.FindControl<Border>("UserEditPopup");
        if (popup != null)
            popup.IsVisible = false;
    }
    
    private void OnSaveUserEditClick(object? sender, RoutedEventArgs e)
    {
        var popup = this.FindControl<Border>("UserEditPopup");
        var textBox = this.FindControl<TextBox>("UserEditTextBox");
        var errorText = this.FindControl<TextBlock>("UserEditErrorText");
        
        if (popup == null || textBox == null || errorText == null || _currentUser == null)
            return;
        
        var value = textBox.Text?.Trim();
        
        // Validate input
        if (string.IsNullOrWhiteSpace(value))
        {
            errorText.Text = "Value cannot be empty";
            errorText.IsVisible = true;
            return;
        }
        
        bool isValid = true;
        
        // Field-specific validation
        switch (_currentEditField)
        {
            case "Email":
                // Simple email validation
                if (!value.Contains("@") || !value.Contains("."))
                {
                    errorText.Text = "Please enter a valid email address";
                    errorText.IsVisible = true;
                    isValid = false;
                }
                break;
            case "Phone":
                // Simple phone validation - allow numbers, +, -, and spaces
                if (!System.Text.RegularExpressions.Regex.IsMatch(value, @"^[0-9\+\-\s]+$"))
                {
                    errorText.Text = "Please enter a valid phone number";
                    errorText.IsVisible = true;
                    isValid = false;
                }
                break;
        }
        
        if (!isValid) return;
        
        // Update user object
        switch (_currentEditField)
        {
            case "UserName":
                _currentUser.UserName = value;
                break;
            case "Email":
                _currentUser.Email = value;
                break;
            case "Phone":
                // Phone property not available in User class
                break;
        }
        
        // In a real app, we would save changes to database using controller
        // For now, just update the display
        UpdateUserProfileDisplay();
        popup.IsVisible = false;
    }
    
    // Password change handlers
    private void OnChangePasswordClick(object? sender, RoutedEventArgs e)
    {
        var popup = this.FindControl<Border>("ChangePasswordPopup");
        var currentPasswordBox = this.FindControl<TextBox>("CurrentPasswordBox");
        var newPasswordBox = this.FindControl<TextBox>("NewPasswordBox");
        var confirmPasswordBox = this.FindControl<TextBox>("ConfirmPasswordBox");
        var errorText = this.FindControl<TextBlock>("PasswordChangeErrorText");
        
        if (popup == null || currentPasswordBox == null || newPasswordBox == null || 
            confirmPasswordBox == null || errorText == null)
            return;
        
        // Reset fields
        currentPasswordBox.Text = string.Empty;
        newPasswordBox.Text = string.Empty;
        confirmPasswordBox.Text = string.Empty;
        errorText.IsVisible = false;
        
        popup.IsVisible = true;
    }
    
    private void OnCancelPasswordChangeClick(object? sender, RoutedEventArgs e)
    {
        var popup = this.FindControl<Border>("ChangePasswordPopup");
        if (popup != null)
            popup.IsVisible = false;
    }
    
    private void OnSavePasswordClick(object? sender, RoutedEventArgs e)
    {
        var popup = this.FindControl<Border>("ChangePasswordPopup");
        var currentPasswordBox = this.FindControl<TextBox>("CurrentPasswordBox");
        var newPasswordBox = this.FindControl<TextBox>("NewPasswordBox");
        var confirmPasswordBox = this.FindControl<TextBox>("ConfirmPasswordBox");
        var errorText = this.FindControl<TextBlock>("PasswordChangeErrorText");
        
        if (popup == null || currentPasswordBox == null || newPasswordBox == null || 
            confirmPasswordBox == null || errorText == null || _currentUser == null)
            return;
        
        var currentPassword = currentPasswordBox.Text;
        var newPassword = newPasswordBox.Text;
        var confirmPassword = confirmPasswordBox.Text;
        
        // Validate input
        if (string.IsNullOrWhiteSpace(currentPassword))
        {
            errorText.Text = "Current password is required";
            errorText.IsVisible = true;
            return;
        }
        
        if (string.IsNullOrWhiteSpace(newPassword))
        {
            errorText.Text = "New password is required";
            errorText.IsVisible = true;
            return;
        }
        
        if (newPassword != confirmPassword)
        {
            errorText.Text = "New password and confirmation do not match";
            errorText.IsVisible = true;
            return;
        }
        
        if (newPassword.Length < 6)
        {
            errorText.Text = "Password must be at least 6 characters";
            errorText.IsVisible = true;
            return;
        }
        
        // In a real app, we would verify current password and update password
        // For now, just hide the popup
        popup.IsVisible = false;
    }
    
    // Change from public to protected
    protected override void OnOpened(EventArgs e)
    {
        base.OnOpened(e);
        
        // Hide the sidebar menu by default and show welcome page first
        if (_sidebarMenu != null)
            _sidebarMenu.IsVisible = false;
        
        // Show welcome page on startup
        ShowWelcomePage();
        
        // Update the user profile display on window opening
        UpdateUserProfileDisplay();
    }
}