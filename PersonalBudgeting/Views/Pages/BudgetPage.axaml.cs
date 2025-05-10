using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Media;
using Avalonia.Markup.Xaml;
using Controller;
using Model.Entities;
using PersonalBudgeting.Views.Pages;

namespace PersonalBudgeting.Views.Pages;

// Extension method for more reliable control lookup
public static class ControlExtensions
{
    public static T? Get<T>(this Control control, string name) where T : Control
    {
        try
        {
            return control.FindControl<T>(name);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error finding control {name}: {ex.Message}");
            return null;
        }
    }
}

public partial class BudgetPage : UserControl
{
    private readonly BudgetController? _budgetController;
    private readonly UserController? _userController;
    private readonly User? _currentUser;
    private readonly List<Budget>? _userBudgets;
    private readonly List<Expense>? _userExpenses;
    private string _searchText = string.Empty;
    private string _selectedStatus = "All Status";
    private StackPanel? _budgetItemsPanel;
    private TextBox? _searchBox;
    private Border? _addBudgetPopup;
    private TextBlock? _addBudgetErrorText;
    private TextBlock? _addBudgetTitle;
    private Button? _saveBudgetButton;
    private TextBox? _budgetNameInput;
    private TextBox? _budgetAmountInput;
    private TextBlock? _totalBudgetText;
    private TextBlock? _remainingBudgetText;
    private ComboBox? _statusFilterComboBox;

    // ViewModel for budget items in the grid
    public class BudgetViewModel
    {
        public int Id { get; set; }
        public string Category { get; set; } = string.Empty;
        public string Budget { get; set; } = string.Empty;
        public string Spent { get; set; } = string.Empty;
        public string Remaining { get; set; } = string.Empty;
        public double Progress { get; set; }
        public string ProgressText { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusColor { get; set; } = string.Empty;
        public bool IsWarning { get; set; }
        public bool IsNegative { get; set; }
    }

    public BudgetPage()
    {
        InitializeComponent();

        // Initialize controllers
        _budgetController = new BudgetController();
        _userController = new UserController();
        _currentUser = new User { Id = 1, UserName = "Test User" };
        _userBudgets = new List<Budget>();
        _userExpenses = new List<Expense>();
        
        // Load initial data
        LoadData();
    }

    public BudgetPage(
        BudgetController budgetController,
        UserController userController,
        User currentUser)
    {
        // First set the controllers and user properties before InitializeComponent
        _budgetController = budgetController ?? throw new ArgumentNullException(nameof(budgetController));
        _userController = userController ?? throw new ArgumentNullException(nameof(userController));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        
        // Initialize to empty lists - will be populated in LoadData
        _userBudgets = new List<Budget>();
        _userExpenses = new List<Expense>();
        
        // Initialize the component first without trying anything else
        AvaloniaXamlLoader.Load(this);
        
        // Delay connecting to controls to ensure UI is ready
        Avalonia.Threading.Dispatcher.UIThread.Post(() => 
        {
            try
            {
                // Connect to UI controls
                FindAndConnectControls();
                
                // Hide the add budget popup initially
                if (_addBudgetPopup != null)
                {
                    _addBudgetPopup.IsVisible = false;
                }
                
                // Load data on UI thread
                LoadDataAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error during UI initialization: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        });
    }

    private async void LoadData()
    {
        await LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        try
        {
            // Ensure we're on the UI thread
            await Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
            {
                if (_currentUser != null && _userController != null)
                {
                    // Load budgets
                    var budgetsResult = await _userController.TryGetUserBudgets(_currentUser);
                    if (budgetsResult.Success && budgetsResult.Budgets != null)
                    {
                        _userBudgets?.Clear();
                        foreach (var budget in budgetsResult.Budgets)
                        {
                            _userBudgets?.Add(budget);
                        }
                    }
                    
                    // Load expenses
                    var expensesResult = await _userController.TryGetUserExpenses(_currentUser);
                    if (expensesResult.Success && expensesResult.Expenses != null)
                    {
                        _userExpenses?.Clear();
                        foreach (var expense in expensesResult.Expenses)
                        {
                            _userExpenses?.Add(expense);
                        }
                    }
                    
                    // Update UI
                    UpdateSummaryCards();
                    UpdateBudgetOverview();
                    UpdateBudgetGrid();
                }
            });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading budget data: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    private void UpdateSummaryCards()
    {
        try
        {
            // Ensure we're on the UI thread
            if (!Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(UpdateSummaryCards);
                return;
            }

            // Calculate total budget
            var totalBudget = _userBudgets != null ? (double)_userBudgets.Sum(b => b.TotalAmountRequired) : 0;
            var totalBudgetText = this.Get<TextBlock>("TotalBudgetText");
            if (totalBudgetText != null)
            {
                totalBudgetText.Text = totalBudget.ToString("C");
            }

            // Calculate budget used (total expenses)
            var totalExpenses = _userExpenses != null ? (double)_userExpenses.Sum(e => e.RequiredAmount) : 0;
            var budgetUsedText = this.Get<TextBlock>("BudgetUsedText");
            if (budgetUsedText != null)
            {
                budgetUsedText.Text = totalExpenses.ToString("C");
            }

            // Calculate percentage of budget used
            var budgetUsedPercentage = totalBudget > 0 ? (totalExpenses / totalBudget) * 100 : 0;
            var budgetUsedPercentageText = this.Get<TextBlock>("BudgetUsedPercentageText");
            if (budgetUsedPercentageText != null)
            {
                budgetUsedPercentageText.Text = $"{budgetUsedPercentage:F1}%";
            }

            // Calculate remaining budget
            var remainingBudget = totalBudget - totalExpenses;
            var remainingBudgetText = this.Get<TextBlock>("RemainingBudgetText");
            if (remainingBudgetText != null)
            {
                remainingBudgetText.Text = remainingBudget.ToString("C");
            }

            // Update the status text
            var daysRemainingText = this.Get<TextBlock>("DaysRemainingText");
            if (daysRemainingText != null)
            {
                if (remainingBudget < 0)
                {
                    daysRemainingText.Text = "Over Budget";
                }
                else if (remainingBudget < (totalBudget * 0.2))
                {
                    daysRemainingText.Text = "Low Budget";
                }
                else
                {
                    daysRemainingText.Text = "Budget Available";
                }
            }
            
            Console.WriteLine("Summary cards updated successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating summary cards: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    private void UpdateBudgetOverview()
    {
        try
        {
            // Ensure we're on the UI thread
            if (!Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(UpdateBudgetOverview);
                return;
            }

            // Calculate total budget
            var totalBudget = _userBudgets != null ? (double)_userBudgets.Sum(b => b.TotalAmountRequired) : 0;
            
            // Calculate total expenses
            var totalExpenses = _userExpenses != null ? (double)_userExpenses.Sum(e => e.RequiredAmount) : 0;
            
            // Calculate percentage of budget used
            var budgetUsedPercentage = totalBudget > 0 ? (totalExpenses / totalBudget) * 100 : 0;
            
            // Calculate remaining budget
            var remainingBudget = totalBudget - totalExpenses;
            
            // Update progress bar
            var overallProgressBar = this.Get<ProgressBar>("OverallProgressBar");
            if (overallProgressBar != null)
            {
                overallProgressBar.Value = Math.Min(budgetUsedPercentage, 100);
                
                // Update progress bar color based on percentage
                overallProgressBar.Classes.Clear();
                
                if (budgetUsedPercentage > 100)
                {
                    overallProgressBar.Classes.Add("danger");
                }
                else if (budgetUsedPercentage > 80)
                {
                    overallProgressBar.Classes.Add("warning");
                }
            }
            
            // Update overview text values
            var overallProgressText = this.Get<TextBlock>("OverallProgressText");
            if (overallProgressText != null)
            {
                overallProgressText.Text = $"{budgetUsedPercentage:F1}%";
            }
            
            var chartBudgetText = this.Get<TextBlock>("ChartBudgetText");
            if (chartBudgetText != null)
            {
                chartBudgetText.Text = totalBudget.ToString("C");
            }
            
            var chartSpentText = this.Get<TextBlock>("ChartSpentText");
            if (chartSpentText != null)
            {
                chartSpentText.Text = totalExpenses.ToString("C");
            }
            
            var chartRemainingText = this.Get<TextBlock>("ChartRemainingText");
            if (chartRemainingText != null)
            {
                chartRemainingText.Text = remainingBudget.ToString("C");
            }
            
            Console.WriteLine("Budget overview updated successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating budget overview: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    private void UpdateBudgetGrid()
    {
        try
        {
            // Ensure we're on the UI thread
            if (!Avalonia.Threading.Dispatcher.UIThread.CheckAccess())
            {
                Avalonia.Threading.Dispatcher.UIThread.Post(UpdateBudgetGrid);
                return;
            }

            // Get the StackPanel that will hold the budget items
            var budgetItemsPanel = this.Get<StackPanel>("BudgetItemsPanel");
            if (budgetItemsPanel == null)
            {
                Console.WriteLine("Could not find BudgetItemsPanel");
                return;
            }
            
            // Clear existing items
            budgetItemsPanel.Children.Clear();

            // Get total expenses by category (using double for calculations)
            var expensesByCategory = _userExpenses != null 
                ? _userExpenses.GroupBy(e => e.ExpenseName)
                             .ToDictionary(
                                 g => g.Key, 
                                 g => (double)g.Sum(e => e.RequiredAmount)
                             )
                : new Dictionary<string, double>();

            // Process budget items
            var budgetItems = _userBudgets != null 
                ? _userBudgets.Select(b => 
                    {
                        // Convert to double to avoid decimal/double conversion issues
                        double budgetAmount = (double)b.TotalAmountRequired;
                        double spentAmount = expensesByCategory.GetValueOrDefault(b.BudgetName, 0);
                        double remainingAmount = budgetAmount - spentAmount;
                        double progress = budgetAmount > 0 ? (spentAmount / budgetAmount) * 100 : 0;
                        
                        return new BudgetViewModel
                        {
                            Id = b.Id,
                            Category = b.BudgetName,
                            Budget = budgetAmount.ToString("C"),
                            Spent = spentAmount.ToString("C"),
                            Remaining = remainingAmount.ToString("C"),
                            Progress = progress,
                            ProgressText = $"{progress:F1}%",
                            Status = GetBudgetStatus(spentAmount, budgetAmount),
                            StatusColor = GetStatusColor(spentAmount, budgetAmount),
                            IsWarning = budgetAmount > 0 && spentAmount > budgetAmount * 0.8 && spentAmount <= budgetAmount,
                            IsNegative = budgetAmount > 0 && spentAmount > budgetAmount
                        };
                    })
                    .Where(b => string.IsNullOrEmpty(_searchText) || 
                               b.Category.Contains(_searchText, StringComparison.OrdinalIgnoreCase))
                    .Where(b => _selectedStatus == "All Status" || 
                               (_selectedStatus == "On Track" && !b.IsWarning && !b.IsNegative) ||
                               (_selectedStatus == "Warning" && b.IsWarning) ||
                               (_selectedStatus == "Over Budget" && b.IsNegative))
                    .OrderByDescending(b => b.Progress)
                    .ToList()
                : new List<BudgetViewModel>();

            Console.WriteLine($"Found {budgetItems.Count} budget items to display");

            // Create UI elements for each budget item
            foreach (var budgetItem in budgetItems)
            {
                // Create a grid for the row
                var rowGrid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitions("*,120,120,120,150,120,100"),
                    Height = 50
                };
                
                // Add a separator at the bottom of each row
                var separator = new Separator
                {
                    Height = 1,
                    Margin = new Avalonia.Thickness(0, 49, 0, 0),
                    Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#20FFFFFF"))
                };
                rowGrid.Children.Add(separator);
                
                // Create and add each column
                AddColumnToGrid(rowGrid, budgetItem);
                
                // Add the row to the panel
                budgetItemsPanel.Children.Add(rowGrid);
            }

            Console.WriteLine("Budget grid updated successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating budget grid: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    private void AddColumnToGrid(Grid rowGrid, BudgetViewModel budgetItem)
    {
        try
        {
            // Category
            var categoryText = new TextBlock
            {
                Text = budgetItem.Category,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Margin = new Avalonia.Thickness(10, 0, 0, 0)
            };
            Grid.SetColumn(categoryText, 0);
            rowGrid.Children.Add(categoryText);
            
            // Budget
            var budgetText = new TextBlock
            {
                Text = budgetItem.Budget,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
            Grid.SetColumn(budgetText, 1);
            rowGrid.Children.Add(budgetText);
            
            // Spent
            var spentText = new TextBlock
            {
                Text = budgetItem.Spent,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
            Grid.SetColumn(spentText, 2);
            rowGrid.Children.Add(spentText);
            
            // Remaining
            var remainingText = new TextBlock
            {
                Text = budgetItem.Remaining,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
            Grid.SetColumn(remainingText, 3);
            rowGrid.Children.Add(remainingText);
            
            // Progress
            var progressGrid = new Grid
            {
                RowDefinitions = new RowDefinitions("Auto,Auto"),
                Margin = new Avalonia.Thickness(5, 8, 5, 8),
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center
            };
            
            var progressBar = new ProgressBar
            {
                Value = budgetItem.Progress,
                Minimum = 0,
                Maximum = 100,
                Height = 8
            };
            
            // Add appropriate classes based on status
            if (budgetItem.IsNegative)
            {
                progressBar.Classes.Add("danger");
            }
            else if (budgetItem.IsWarning)
            {
                progressBar.Classes.Add("warning");
            }
            
            var progressText = new TextBlock
            {
                Text = budgetItem.ProgressText,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Right,
                Margin = new Avalonia.Thickness(0, 2, 0, 0),
                FontSize = 11
            };
            
            Grid.SetRow(progressBar, 0);
            Grid.SetRow(progressText, 1);
            progressGrid.Children.Add(progressBar);
            progressGrid.Children.Add(progressText);
            
            Grid.SetColumn(progressGrid, 4);
            rowGrid.Children.Add(progressGrid);
            
            // Status
            var statusBorder = new Border
            {
                CornerRadius = new Avalonia.CornerRadius(4),
                Padding = new Avalonia.Thickness(5, 2, 5, 2),
                Margin = new Avalonia.Thickness(10, 0, 10, 0),
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Background = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse(budgetItem.StatusColor))
            };
            
            var statusText = new TextBlock
            {
                Text = budgetItem.Status,
                Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.White),
                FontSize = 12,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            };
            
            statusBorder.Child = statusText;
            Grid.SetColumn(statusBorder, 5);
            rowGrid.Children.Add(statusBorder);
            
            // Actions
            var actionsPanel = new StackPanel
            {
                Orientation = Avalonia.Layout.Orientation.Horizontal,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                Spacing = 5
            };
            
            var editButton = new Button
            {
                Content = "Edit",
                Padding = new Avalonia.Thickness(5),
                Tag = budgetItem.Id
            };
            
            editButton.Click += OnEditBudgetClick;
            
            var deleteButton = new Button
            {
                Content = "Delete",
                Padding = new Avalonia.Thickness(5),
                Tag = budgetItem.Id
            };
            
            deleteButton.Click += OnDeleteBudgetClick;
            
            actionsPanel.Children.Add(editButton);
            actionsPanel.Children.Add(deleteButton);
            
            Grid.SetColumn(actionsPanel, 6);
            rowGrid.Children.Add(actionsPanel);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error adding columns to grid: {ex.Message}");
        }
    }

    private string GetBudgetStatus(double spent, double budget)
    {
        if (budget <= 0) return "No Budget";
        
        var percentage = (spent / budget) * 100;
        return percentage switch
        {
            > 100 => "Over Budget",
            > 80 => "Warning",
            _ => "On Track"
        };
    }

    private string GetStatusColor(double spent, double budget)
    {
        if (budget <= 0) return "#757575"; // Gray for no budget
        
        var percentage = (spent / budget) * 100;
        
        if (percentage > 100)
        {
            return "#FF5252"; // Red for over budget
        }
        else if (percentage > 80)
        {
            return "#FFC400"; // Amber for warning
        }
        else
        {
            return "#4CAF50"; // Green for on track
        }
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            _searchText = textBox.Text ?? string.Empty;
            UpdateBudgetGrid();
        }
    }

    private void OnStatusFilterChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            _selectedStatus = selectedItem.Content?.ToString() ?? "All Status";
            UpdateBudgetGrid();
        }
    }

    private void OnCreateBudgetClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            Console.WriteLine("Create Budget button clicked");
            
            // Show the budget creation popup
            var addBudgetPopup = this.FindControl<Border>("AddBudgetPopup");
            if (addBudgetPopup != null)
            {
                // Ensure the popup is visible
                addBudgetPopup.IsVisible = true;
                Console.WriteLine("Budget popup should now be visible");
            }
            else
            {
                Console.WriteLine("Failed to find AddBudgetPopup control");
            }
            
            // Reset form controls
            var addBudgetErrorText = this.FindControl<TextBlock>("AddBudgetErrorText");
            var budgetNameInput = this.FindControl<TextBox>("BudgetNameInput");
            var budgetAmountInput = this.FindControl<TextBox>("BudgetAmountInput");
            
            // Clear form fields
            if (budgetNameInput != null) budgetNameInput.Text = string.Empty;
            if (budgetAmountInput != null) budgetAmountInput.Text = string.Empty;
            
            // Hide error message
            if (addBudgetErrorText != null)
            {
                addBudgetErrorText.IsVisible = false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error showing budget popup: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    private void OnEditBudgetClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int budgetId && _userBudgets != null)
        {
            var budget = _userBudgets.FirstOrDefault(b => b.Id == budgetId);
            if (budget != null)
            {
                // For now, just log the action
                Console.WriteLine($"Edit budget request: {budget.BudgetName} (${budget.TotalAmountRequired})");
            }
        }
    }

    private void OnDeleteBudgetClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int budgetId && _userBudgets != null)
        {
            var budget = _userBudgets.FirstOrDefault(b => b.Id == budgetId);
            if (budget != null)
            {                
                // For now, just log the action
                Console.WriteLine($"Delete budget request: {budget.BudgetName} (${budget.TotalAmountRequired})");
            }
        }
    }
    
    private void OnBackClick(object? sender, RoutedEventArgs e)
    {
        // Navigate back to the dashboard
        if (this.Parent is ContentControl contentControl)
        {
            contentControl.Content = new HomePage();
        }
    }

    private void OnCancelAddBudgetClick(object sender, RoutedEventArgs e)
    {
        var addBudgetPopup = this.FindControl<Border>("AddBudgetPopup");
        if (addBudgetPopup != null)
        {
            addBudgetPopup.IsVisible = false;
        }
    }

    private async void OnSaveBudgetClick(object sender, RoutedEventArgs e)
    {
        try
        {
            Console.WriteLine("Save Budget button clicked");
            
            // Get references to popup controls
            var addBudgetPopup = this.Get<Border>("AddBudgetPopup");
            var addBudgetErrorText = this.Get<TextBlock>("AddBudgetErrorText");
            var budgetNameInput = this.Get<TextBox>("BudgetNameInput");
            var budgetAmountInput = this.Get<TextBox>("BudgetAmountInput");
            
            // Validate inputs
            if (budgetNameInput == null || budgetAmountInput == null || 
                addBudgetErrorText == null || _budgetController == null || _currentUser == null)
            {
                Console.WriteLine("Missing controls or controllers");
                return;
            }
            
            // Get input values
            string budgetName = budgetNameInput.Text?.Trim() ?? string.Empty;
            string amountText = budgetAmountInput.Text?.Trim() ?? string.Empty;
            
            // Validate budget name
            if (string.IsNullOrWhiteSpace(budgetName))
            {
                addBudgetErrorText.Text = "Budget category name is required";
                addBudgetErrorText.IsVisible = true;
                return;
            }
            
            // Validate amount
            if (string.IsNullOrWhiteSpace(amountText) || !decimal.TryParse(amountText, out decimal amount))
            {
                addBudgetErrorText.Text = "Please enter a valid amount";
                addBudgetErrorText.IsVisible = true;
                return;
            }
            
            if (amount <= 0)
            {
                addBudgetErrorText.Text = "Amount must be greater than zero";
                addBudgetErrorText.IsVisible = true;
                return;
            }
            
            // Create budget object - only set required fields
            var budget = new Budget
            {
                UserId = _currentUser.Id,
                BudgetName = budgetName,
                TotalAmountRequired = amount
            };
            
            // Add budget to database
            Console.WriteLine($"Creating budget category: {budget.BudgetName} - {budget.TotalAmountRequired:C}");
            var result = await _budgetController.TryAddBudget(budget);
            
            if (result.Success)
            {
                Console.WriteLine("Budget category created successfully");
                
                // Hide the popup
                if (addBudgetPopup != null)
                {
                    addBudgetPopup.IsVisible = false;
                }
                
                // Reload budget data
                await LoadDataAsync();
            }
            else
            {
                // Get error message from the errors list or use a default message
                string errorMessage = (result.errors != null && result.errors.Count > 0) 
                    ? string.Join(", ", result.errors) 
                    : "Failed to create budget category";
                    
                // Show error message
                addBudgetErrorText.Text = errorMessage;
                addBudgetErrorText.IsVisible = true;
                Console.WriteLine($"Error creating budget category: {errorMessage}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving budget: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            
            // Show error message in UI
            var addBudgetErrorText = this.Get<TextBlock>("AddBudgetErrorText");
            if (addBudgetErrorText != null)
            {
                addBudgetErrorText.Text = $"An error occurred: {ex.Message}";
                addBudgetErrorText.IsVisible = true;
            }
        }
    }

    private void InitializeComponent()
    {
        try
        {
            // Just load the XAML without trying to find controls here
            AvaloniaXamlLoader.Load(this);
            Console.WriteLine("BudgetPage XAML loaded successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in InitializeComponent: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }

    private void FindAndConnectControls()
    {
        try
        {
            Console.WriteLine("Attempting to find and connect controls in BudgetPage");

            // Use direct access approach instead of FindControl when possible
            _budgetItemsPanel = this.Get<StackPanel>("BudgetItemsPanel");
            Console.WriteLine($"_budgetItemsPanel found: {_budgetItemsPanel != null}");
            
            _searchBox = this.Get<TextBox>("SearchBox");
            Console.WriteLine($"_searchBox found: {_searchBox != null}");
            
            _addBudgetPopup = this.Get<Border>("AddBudgetPopup");
            Console.WriteLine($"_addBudgetPopup found: {_addBudgetPopup != null}");
            
            _addBudgetErrorText = this.Get<TextBlock>("AddBudgetErrorText");
            Console.WriteLine($"_addBudgetErrorText found: {_addBudgetErrorText != null}");
            
            _addBudgetTitle = this.Get<TextBlock>("AddBudgetTitle");
            Console.WriteLine($"_addBudgetTitle found: {_addBudgetTitle != null}");
            
            _saveBudgetButton = this.Get<Button>("SaveBudgetButton");
            Console.WriteLine($"_saveBudgetButton found: {_saveBudgetButton != null}");
            
            _budgetNameInput = this.Get<TextBox>("BudgetNameInput");
            Console.WriteLine($"_budgetNameInput found: {_budgetNameInput != null}");
            
            _budgetAmountInput = this.Get<TextBox>("BudgetAmountInput");
            Console.WriteLine($"_budgetAmountInput found: {_budgetAmountInput != null}");
            
            _totalBudgetText = this.Get<TextBlock>("TotalBudgetText");
            Console.WriteLine($"_totalBudgetText found: {_totalBudgetText != null}");
            
            _remainingBudgetText = this.Get<TextBlock>("RemainingBudgetText");
            Console.WriteLine($"_remainingBudgetText found: {_remainingBudgetText != null}");
            
            _statusFilterComboBox = this.Get<ComboBox>("StatusFilterComboBox");
            Console.WriteLine($"_statusFilterComboBox found: {_statusFilterComboBox != null}");
            
            // Connect event handlers - only connect if control is found
            if (_searchBox != null)
            {
                _searchBox.TextChanged -= OnSearchTextChanged; // Remove any existing handler first
                _searchBox.TextChanged += OnSearchTextChanged;
                Console.WriteLine("Connected _searchBox.TextChanged event handler");
            }
            
            if (_statusFilterComboBox != null)
            {
                _statusFilterComboBox.SelectionChanged -= OnStatusFilterChanged; // Remove any existing handler first
                _statusFilterComboBox.SelectionChanged += OnStatusFilterChanged;
                Console.WriteLine("Connected _statusFilterComboBox.SelectionChanged event handler");
            }
            
            // Ensure the add budget popup is initially hidden
            if (_addBudgetPopup != null)
            {
                _addBudgetPopup.IsVisible = false;
                Console.WriteLine("Set _addBudgetPopup.IsVisible = false");
            }
            
            Console.WriteLine("Successfully completed FindAndConnectControls in BudgetPage");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error finding and connecting controls: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
    }
}
