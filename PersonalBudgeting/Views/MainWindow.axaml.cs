using Avalonia.Controls;
using FluentAvalonia.UI.Controls;
using PersonalBudgeting.Views.Pages;

namespace PersonalBudgeting.Views;

public partial class MainWindow : Window
{
<<<<<<< Updated upstream
    private readonly Frame _contentFrame;
=======
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
    private readonly TextBlock? _userEmailText;
    private string _currentEditField = string.Empty;
>>>>>>> Stashed changes

    public MainWindow()
    {
        InitializeComponent();
        
        _contentFrame = this.FindControl<Frame>("ContentFrame");
        var navView = this.FindControl<NavigationView>("NavView");
        
<<<<<<< Updated upstream
        if (navView != null)
        {
            navView.SelectionChanged += NavView_SelectionChanged;
=======
        // Remove the test user initialization - we'll get real users from the database
        _currentUser = null;
        
        // Find controls
        _pageContent = this.FindControl<ContentControl>("PageContent") ?? 
            throw new InvalidOperationException("PageContent not found");
        _sidebarMenu = this.FindControl<ScrollViewer>("SidebarMenu") ?? 
            throw new InvalidOperationException("SidebarMenu not found");
        _userInitials = this.FindControl<TextBlock>("UserInitials");
        _userNameText = this.FindControl<TextBlock>("UserNameText");
        _userEmailText = this.FindControl<TextBlock>("UserEmailText");
        
        // Set username to indicate login is required
        if (_userNameText != null)
        {
            _userNameText.Text = "Guest User";
        }
        
        if (_userEmailText != null)
        {
            _userEmailText.Text = "Please login";
>>>>>>> Stashed changes
        }

        // Navigate to home page by default
        if (_contentFrame != null)
        {
<<<<<<< Updated upstream
            _contentFrame.Navigate(typeof(HomePage));
=======
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
                
            }
            else
            {
            }
            
            // Always proceed to load the home page for valid users
            LoadHomePage();
        }
        catch (Exception ex)
        {
            ShowWelcomePage();
>>>>>>> Stashed changes
        }
    }

    private void NavView_SelectionChanged(object? sender, NavigationViewSelectionChangedEventArgs e)
    {
        if (e.SelectedItem is NavigationViewItem item && _contentFrame != null)
        {
<<<<<<< Updated upstream
            switch (item.Tag?.ToString()?.ToLower())
=======
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
                if (_userEmailText != null)
                    _userEmailText.Text = _currentUser.Email ?? "No email";
                
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
>>>>>>> Stashed changes
            {
                case "home":
                    _contentFrame.Navigate(typeof(HomePage));
                    break;
                case "profile":
                    _contentFrame.Navigate(typeof(ProfilePage));
                    break;
                case "income":
                    _contentFrame.Navigate(typeof(IncomePage));
                    break;
                case "expenses":
                    _contentFrame.Navigate(typeof(ExpensesPage));
                    break;
                case "budget":
<<<<<<< Updated upstream
                    _contentFrame.Navigate(typeof(BudgetPage));
                    break;
                case "reminders":
                    _contentFrame.Navigate(typeof(RemindersPage));
=======
                    var (budgetsSuccess, userBudgets, budgetErrors) = await _userController.TryGetUserBudgets(_currentUser);
                    var (expSuccess, expensesForBudget, expErrors) = await _userController.TryGetUserExpenses(_currentUser);
                    
                    try
                    {
                        var budgetPage = new BudgetPage(
                            _budgetController,
                            _userController,
                            _currentUser);
                        
                        _pageContent.Content = budgetPage;
                    }
                    catch (Exception ex)
                    {
                        
                        // Fallback to home page if budget page fails to load
                        LoadHomePage();
                    }
>>>>>>> Stashed changes
                    break;
                case "settings":
                    _contentFrame.Navigate(typeof(SettingsPage));
                    break;
            }
        }
<<<<<<< Updated upstream
=======
        catch (Exception ex)
        {
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
    
    // Method to update the user profile display
    private void UpdateUserProfileDisplay()
    {
        if (_currentUser != null)
        {
            if (_userNameText != null)
            {
                _userNameText.Text = _currentUser.UserName ?? "User";
            }
            
            if (_userEmailText != null)
            {
                _userEmailText.Text = _currentUser.Email ?? "No email";
            }
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
>>>>>>> Stashed changes
    }
}
