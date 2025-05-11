using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Controller;
using Model.Entities;
using PersonalBudgeting.Views.Pages;

namespace PersonalBudgeting.Views.Pages;

public partial class IncomePage : UserControl
{
    private readonly IncomeController _incomeController;
    private readonly UserController _userController;
    private readonly User _currentUser;
    private List<Income> _userIncomes;

    private TextBlock? _welcomeMessage;
    private TextBlock? _totalIncomeText;
    private TextBlock? _avgIncomeText;
    private TextBlock? _incomeSourcesText;
    private TextBox? _searchBox;
    private ComboBox? _categoryFilter;
    private Panel? _incomeListPanel;

    public IncomePage()
    {
        InitializeComponent();
        
        // Initialize controllers
        _incomeController = new IncomeController();
        _userController = new UserController();
        
        // In a real app, this would be from authentication
        _currentUser = new User { Id = 1, UserName = "testuser" };
        _userIncomes = new List<Income>
        {
            new Income { Id = 1, UserId = 1, IncomeSourceName = "Salary", Amount = 3000, IncomeDate = DateTime.Now.AddDays(-5) },
            new Income { Id = 2, UserId = 1, IncomeSourceName = "Freelance", Amount = 500, IncomeDate = DateTime.Now.AddDays(-10) },
            new Income { Id = 3, UserId = 1, IncomeSourceName = "Investments", Amount = 200, IncomeDate = DateTime.Now.AddDays(-15) }
        };
        
        // Get control references
        GetControlReferences();
        
        // Set up event handlers
        if (_searchBox != null)
            _searchBox.TextChanged += OnSearchTextChanged;
            
        if (_categoryFilter != null)
            _categoryFilter.SelectionChanged += OnCategoryFilterChanged;
            
        // Load the data
        LoadData();
    }

    public IncomePage(IncomeController incomeController, UserController userController, User user, List<Income> userIncomes)
    {
        InitializeComponent();
        
        _incomeController = incomeController;
        _userController = userController;
        _currentUser = user;
        _userIncomes = userIncomes;
        
        // Get control references
        GetControlReferences();
        
        // Set up event handlers
        if (_searchBox != null)
            _searchBox.TextChanged += OnSearchTextChanged;
            
        if (_categoryFilter != null)
            _categoryFilter.SelectionChanged += OnCategoryFilterChanged;
            
        // Load the data
        LoadData();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void GetControlReferences()
    {
        _welcomeMessage = this.FindControl<TextBlock>("WelcomeMessage");
        _totalIncomeText = this.FindControl<TextBlock>("TotalIncomeText");
        _avgIncomeText = this.FindControl<TextBlock>("AvgIncomeText");
        _incomeSourcesText = this.FindControl<TextBlock>("IncomeSourcesText");
        _searchBox = this.FindControl<TextBox>("SearchBox");
        _categoryFilter = this.FindControl<ComboBox>("CategoryFilter");
        _incomeListPanel = this.FindControl<Panel>("IncomeListPanel");
    }
    
    private void LoadData()
    {
        // Set welcome message
        if (_welcomeMessage != null)
            _welcomeMessage.Text = $"Welcome, {_currentUser.UserName}";
        
        // Update UI components
        UpdateSummaryCards();
        UpdateIncomeGrid();
    }
    
    private async void UpdateSummaryCards()
    {
        try
        {
            // Use the controller method to get total income instead of summing the list
            var (totalSuccess, totalAmount, totalErrors) = await _userController.TryGetTotalUserIncomes(_currentUser.Id);
            
            
            // Calculate average income (if we have incomes)
            var avgIncome = _userIncomes.Any() ? _userIncomes.Average(i => i.Amount) : 0;
            
            // Count unique income sources
            var uniqueSources = _userIncomes.Select(i => i.IncomeSourceName).Distinct().Count();
            
            // Update UI
            if (_totalIncomeText != null)
                _totalIncomeText.Text = totalSuccess ? $"${totalAmount:N2}" : "$0.00";
                
            if (_avgIncomeText != null)
                _avgIncomeText.Text = $"${avgIncome:N2}";
                
            if (_incomeSourcesText != null)
                _incomeSourcesText.Text = uniqueSources.ToString();
                
            if (!totalSuccess && totalErrors.Count > 0)
            {
            }
        }
        catch (Exception ex)
        {
        }
    }
    
    private void UpdateIncomeGrid()
    {
        try
        {
            if (_incomeListPanel == null)
                return;
                
            // Clear existing items
            _incomeListPanel.Children.Clear();
            
            // Get search text
            var searchText = _searchBox?.Text?.ToLower() ?? "";
            
            // Filter incomes based on search text
            var filteredIncomes = _userIncomes
                .Where(i => string.IsNullOrEmpty(searchText) || 
                           i.IncomeSourceName.ToLower().Contains(searchText))
                .OrderByDescending(i => i.IncomeDate)
                .ToList();
            
            // Create UI elements for each income
            foreach (var income in filteredIncomes)
            {
                // Create a border for the entire row
                var rowBorder = new Border
                {
                    BorderBrush = new SolidColorBrush(Color.Parse("#333333")),
                    BorderThickness = new Thickness(1, 0, 1, 1),
                    Background = new SolidColorBrush(Color.Parse("#2E2E2E")),
                    Height = 50
                };

                // Create a grid for this row with 4 columns
                var rowGrid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitions("2*,1*,1*,1*")
                };

                // Income Source column
                var sourceText = new TextBlock
                {
                    Text = income.IncomeSourceName,
                    VerticalAlignment = VerticalAlignment.Center,
                    Margin = new Thickness(10, 0, 0, 0)
                };
                Grid.SetColumn(sourceText, 0);
                rowGrid.Children.Add(sourceText);
                
                // Amount column
                var amountText = new TextBlock
                {
                    Text = $"${income.Amount:N2}",
                    Foreground = new SolidColorBrush(Color.Parse("#4CAF50")),
                    FontWeight = FontWeight.SemiBold,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetColumn(amountText, 1);
                rowGrid.Children.Add(amountText);
                
                // Date column
                var dateText = new TextBlock
                {
                    Text = income.IncomeDate.ToString("yyyy-MM-dd"),
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Center
                };
                Grid.SetColumn(dateText, 2);
                rowGrid.Children.Add(dateText);
                
                // Actions column
                var actionPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 5,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    VerticalAlignment = VerticalAlignment.Center
                };
                
                // Edit button
                var editButton = new Button
                {
                    Content = "Edit",
                    Classes = { "actionButton" },
                    Tag = income.Id,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center
                };
                editButton.Click += OnEditIncomeClick;
                
                // Delete button
                var deleteButton = new Button
                {
                    Content = "Delete",
                    Classes = { "actionButton", "deleteButton" },
                    Tag = income.Id,
                    HorizontalContentAlignment = HorizontalAlignment.Center,
                    VerticalContentAlignment = VerticalAlignment.Center
                };
                deleteButton.Click += OnDeleteIncomeClick;
                
                // Add buttons to panel
                actionPanel.Children.Add(editButton);
                actionPanel.Children.Add(deleteButton);
                Grid.SetColumn(actionPanel, 3);
                rowGrid.Children.Add(actionPanel);

                // Add the grid to the border
                rowBorder.Child = rowGrid;
                
                // Add the row to the panel
                _incomeListPanel.Children.Add(rowBorder);
            }
            
            // Display empty state message if no incomes
            if (filteredIncomes.Count == 0)
            {
                var emptyMessage = new TextBlock
                {
                    Text = "No income records found.",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Avalonia.Thickness(0, 20, 0, 0),
                    Opacity = 0.7
                };
                _incomeListPanel.Children.Add(emptyMessage);
            }
        }
        catch (Exception ex)
        {
        }
    }
    
    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        UpdateIncomeGrid();
    }
    
    private void OnCategoryFilterChanged(object? sender, SelectionChangedEventArgs e)
    {
        UpdateIncomeGrid();
    }
    
    private void OnEditIncomeClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int incomeId)
        {
            try
            {
                // Find the income to edit
                var incomeToEdit = _userIncomes.FirstOrDefault(i => i.Id == incomeId);
                if (incomeToEdit != null)
                {
                    // Get the popup and input controls
                    var popup = this.FindControl<Border>("AddIncomePopup");
                    var sourceInput = this.FindControl<TextBox>("IncomeSourceInput");
                    var amountInput = this.FindControl<TextBox>("IncomeAmountInput");
                    var datePicker = this.FindControl<DatePicker>("IncomeDatePicker");
                    var saveButton = this.FindControl<Button>("SaveIncomeButton");
                    var errorText = this.FindControl<TextBlock>("AddIncomeErrorText");
                    var popupTitle = this.FindControl<TextBlock>("AddIncomeTitle");
                    
                    if (popup == null || sourceInput == null || amountInput == null || 
                        datePicker == null || saveButton == null || errorText == null || popupTitle == null)
            return;
                    
                    // Set the current values in the form
                    sourceInput.Text = incomeToEdit.IncomeSourceName;
                    amountInput.Text = incomeToEdit.Amount.ToString();
                    datePicker.SelectedDate = incomeToEdit.IncomeDate;
                    
                    // Change popup title
                    popupTitle.Text = "Edit Income";
                    
                    // Clear any previous error
                    errorText.IsVisible = false;
                    
                    // Store the income ID in the save button's Tag for reference
                    saveButton.Tag = incomeId;
                    
                    // Show the popup
                    popup.IsVisible = true;
                    
                }
            }
            catch (Exception ex)
            {
            }
        }
    }
    
    private async void OnDeleteIncomeClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is Button button && button.Tag is int incomeId)
            {
                // Find the income to delete
                var incomeToDelete = _userIncomes.FirstOrDefault(i => i.Id == incomeId);
                if (incomeToDelete != null)
                {
                    // Delete income using controller
                    var (success, Errors) = await _incomeController.TryDeleteIncome(incomeToDelete);
                    
                    if (success)
                    {
                        // Remove from the local list
                        _userIncomes.Remove(incomeToDelete);
                        
                        // Update UI
                        UpdateSummaryCards();
                        UpdateIncomeGrid();
                    }
                    else
                    {
                        // Show error message (in a real app, you'd show a popup)
                    }
                }
            }
        }
        catch (Exception)
        {
            // Error handling
        }
    }
    
    private void OnAddIncomeClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var popup = this.FindControl<Border>("AddIncomePopup");
            if (popup != null)
            {
                // Reset inputs
                var sourceInput = this.FindControl<TextBox>("IncomeSourceInput");
                var amountInput = this.FindControl<TextBox>("IncomeAmountInput");
                var datePicker = this.FindControl<DatePicker>("IncomeDatePicker");
                var errorText = this.FindControl<TextBlock>("AddIncomeErrorText");
                var popupTitle = this.FindControl<TextBlock>("AddIncomeTitle");
                var saveButton = this.FindControl<Button>("SaveIncomeButton");
                
                if (sourceInput != null)
                    sourceInput.Text = string.Empty;
                
                if (amountInput != null)
                    amountInput.Text = string.Empty;
                
                if (datePicker != null)
                    datePicker.SelectedDate = DateTime.Today;
                
                if (errorText != null)
                    errorText.IsVisible = false;
                
                if (popupTitle != null)
                    popupTitle.Text = "Add New Income";
                
                if (saveButton != null)
                    saveButton.Tag = null; // Clear any previous edit ID
                
                // Show popup
                popup.IsVisible = true;
            }
        }
        catch (Exception)
        {
            // Error handling
        }
    }
    
    private async void OnSaveIncomeClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Get the popup and input controls
            var popup = this.FindControl<Border>("AddIncomePopup");
            var sourceInput = this.FindControl<TextBox>("IncomeSourceInput");
            var amountInput = this.FindControl<TextBox>("IncomeAmountInput");
            var datePicker = this.FindControl<DatePicker>("IncomeDatePicker");
            var saveButton = this.FindControl<Button>("SaveIncomeButton");
            var errorText = this.FindControl<TextBlock>("AddIncomeErrorText");
            
            // Validate that we have all required controls and data
            if (popup == null || sourceInput == null || amountInput == null || 
                datePicker == null || saveButton == null || errorText == null || 
                _incomeController == null || _currentUser == null)
            {
                return;
            }
            
            // Validate source name (required)
            string sourceName = sourceInput.Text?.Trim() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(sourceName))
            {
                errorText.Text = "Income source name is required";
                errorText.IsVisible = true;
                return;
            }
            
            // Validate source name length (max 100 chars)
            if (sourceName.Length > 100)
            {
                errorText.Text = "Income source name cannot exceed 100 characters";
                errorText.IsVisible = true;
                return;
            }
            
            // Validate amount (required and must be a number)
            if (string.IsNullOrWhiteSpace(amountInput.Text) || !decimal.TryParse(amountInput.Text, out decimal amount))
            {
                errorText.Text = "Please enter a valid amount";
                errorText.IsVisible = true;
                return;
            }
            
            // Validate amount is positive
            if (amount <= 0)
            {
                errorText.Text = "Amount must be greater than zero";
                errorText.IsVisible = true;
                return;
            }
            
            // Validate amount max 6 digits
            if (amount.ToString("0").Length > 6)
            {
                errorText.Text = "Amount cannot exceed 6 digits";
                errorText.IsVisible = true;
                return;
            }
            
            // Validate date (required)
            if (datePicker.SelectedDate == null)
            {
                errorText.Text = "Please select a date";
                errorText.IsVisible = true;
                return;
            }
            
            // Check if editing or adding
            bool isEditing = saveButton.Tag is int incomeId && incomeId > 0;
            
            // Create income object
            var income = new Income
            {
                UserId = _currentUser.Id,
                IncomeSourceName = sourceName,
                Amount = amount,
                IncomeDate = datePicker.SelectedDate.Value.DateTime
            };
            
            if (isEditing && saveButton.Tag is int editId)
            {
                // Set ID for update
                income.Id = editId;
                
                var result = await _incomeController.TryUpdateIncome(income);
                if (result.Success)
                {
                    // Hide popup
                    popup.IsVisible = false;
                    
                    // Reload income data
                    await LoadIncomeDataAsync();
                }
                else
                {
                    string errorMessage = (result.errors != null && result.errors.Count > 0) 
                        ? string.Join(", ", result.errors) 
                        : "Failed to update income";
                        
                    errorText.Text = errorMessage;
                    errorText.IsVisible = true;
                }
            }
            else
            {
                // Add new income
                var result = await _incomeController.TryAddIncome(income);
                if (result.Success)
                {
                    // Hide popup
                    popup.IsVisible = false;
                    
                    // Reload income data
                    await LoadIncomeDataAsync();
                }
                else
                {
                    string errorMessage = (result.errors != null && result.errors.Count > 0) 
                        ? string.Join(", ", result.errors) 
                        : "Failed to add income";
                        
                    errorText.Text = errorMessage;
                    errorText.IsVisible = true;
                }
            }
        }
        catch (Exception ex)
        {
            
            // Show error in UI
            var errorText = this.FindControl<TextBlock>("AddIncomeErrorText");
            if (errorText != null)
            {
                errorText.Text = $"An error occurred: {ex.Message}";
                errorText.IsVisible = true;
            }
        }
    }
    
    private async Task LoadIncomeDataAsync()
    {
        try
        {
            if (_currentUser == null || _incomeController == null || _userController == null)
            {
                return;
            }
            
            // Load incomes
            var result = await _userController.TryGetUserIncomes(_currentUser);
            
            if (result.Success)
            {
                _userIncomes = result.Incomes ?? new List<Income>();
                
                // Update UI
                UpdateSummaryCards();
                UpdateIncomeGrid();
            }
            else
            {
            }
        }
        catch (Exception)
        {
            // Error handling
        }
    }
    
    private void OnCancelAddIncomeClick(object? sender, RoutedEventArgs e)
    {
        var popup = this.FindControl<Border>("AddIncomePopup");
        if (popup != null)
        {
            popup.IsVisible = false;
        }
    }
    
    private void OnBackClick(object? sender, RoutedEventArgs e)
    {
        // Navigate back to the dashboard
        var mainWindow = TopLevel.GetTopLevel(this) as MainWindow;
        if (mainWindow != null)
        {
            // Use reflection to access the private/protected NavigateToPage method
            var method = mainWindow.GetType().GetMethod("NavigateToPage", 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Public);
            
            if (method != null)
            {
                method.Invoke(mainWindow, new object[] { "home" });
            }
        }
    }
}

// Helper class for income view models
public class IncomeViewModel
{
    public string Date { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
}
