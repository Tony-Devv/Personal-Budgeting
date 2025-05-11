using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PersonalBudgeting.Views.Pages;

<<<<<<< Updated upstream
=======
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
            return null;
        }
    }
}

>>>>>>> Stashed changes
public partial class BudgetPage : UserControl
{
    public BudgetPage()
    {
        InitializeComponent();
<<<<<<< Updated upstream
=======

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
            
        }
        catch (Exception ex)
        {
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
            
        }
        catch (Exception)
        {
            // Error handling
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

            // Get the budget items panel
            if (_budgetItemsPanel == null)
            {
                _budgetItemsPanel = this.Get<StackPanel>("BudgetItemsPanel");
                if (_budgetItemsPanel == null)
                {
                    return;
                }
            }

            // Clear existing items
            _budgetItemsPanel.Children.Clear();

            // If no budgets, exit
            if (_userBudgets == null || !_userBudgets.Any())
            {
                return;
            }

            // Filter budgets by search text and status
            var filteredBudgets = _userBudgets
                .Where(b => string.IsNullOrEmpty(_searchText) || 
                            b.BudgetName.Contains(_searchText, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (_selectedStatus != "All Status")
            {
                // For each budget, calculate spent amount first, then filter by status
                filteredBudgets = filteredBudgets
                    .Where(b => {
                        decimal spent = _userExpenses != null 
                            ? _userExpenses.Where(e => e.BudgetId == b.Id).Sum(e => e.RequiredAmount) 
                            : 0;
                        return GetBudgetStatus((double)spent, (double)b.TotalAmountRequired) == _selectedStatus;
                    })
                    .ToList();
            }

            // Add a row for each budget
            foreach (var budget in filteredBudgets)
            {
                // Skip if no category or blank
                if (string.IsNullOrWhiteSpace(budget.BudgetName))
                {
                    continue;
                }

                // Get spent amount for this budget
                decimal spent = _userExpenses != null 
                    ? _userExpenses
                        .Where(e => e.BudgetId == budget.Id)
                        .Sum(e => e.RequiredAmount)
                    : 0;

                // Calculate remaining budget
                decimal remaining = budget.TotalAmountRequired - spent;
                
                // Calculate progress percentage
                double progress = budget.TotalAmountRequired > 0 
                    ? Math.Min(100, (double)(spent / budget.TotalAmountRequired * 100)) 
                    : 0;

                // Create budget view model
                var budgetItem = new BudgetViewModel
                {
                    Id = budget.Id,
                    Category = budget.BudgetName,
                    Budget = budget.TotalAmountRequired.ToString("C"),
                    Spent = spent.ToString("C"),
                    Remaining = remaining.ToString("C"),
                    Progress = progress,
                    ProgressText = $"{progress:F1}%",
                    Status = GetBudgetStatus((double)spent, (double)budget.TotalAmountRequired),
                    StatusColor = GetStatusColor((double)spent, (double)budget.TotalAmountRequired),
                    IsWarning = progress > 80 && progress <= 100,
                    IsNegative = progress > 100
                };

                // Create a border for the entire row
                var rowBorder = new Border
                {
                    BorderBrush = new SolidColorBrush(Color.Parse("#333333")),
                    BorderThickness = new Thickness(1, 0, 1, 1),
                    Background = new SolidColorBrush(Color.Parse("#2E2E2E")),
                    Height = 50
                };

                // Create a grid for this row with 7 columns
                var rowGrid = new Grid
                {
                    ColumnDefinitions = new ColumnDefinitions("2*,1*,1*,1*,1*,1*,1*")
                };

                // Add the grid to the border
                rowBorder.Child = rowGrid;

                // Add column data to the grid
                AddColumnToGrid(rowGrid, budgetItem);

                // Add the row to the panel
                _budgetItemsPanel.Children.Add(rowBorder);
            }
        }
        catch (Exception ex)
        {
        }
    }

    private void AddColumnToGrid(Grid rowGrid, BudgetViewModel budgetItem)
    {
        try
        {
            // Column 0: Category Name
            var categoryText = new TextBlock
            {
                Text = budgetItem.Category,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(10, 0, 0, 0)
            };
            Grid.SetColumn(categoryText, 0);
            rowGrid.Children.Add(categoryText);

            // Column 1: Budget Amount
            var budgetText = new TextBlock
            {
                Text = budgetItem.Budget,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Grid.SetColumn(budgetText, 1);
            rowGrid.Children.Add(budgetText);

            // Column 2: Spent Amount
            var spentText = new TextBlock
            {
                Text = budgetItem.Spent,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center
            };
            Grid.SetColumn(spentText, 2);
            rowGrid.Children.Add(spentText);

            // Column 3: Remaining Amount
            var remainingText = new TextBlock
            {
                Text = budgetItem.Remaining,
                VerticalAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                Foreground = budgetItem.IsNegative 
                    ? new SolidColorBrush(Color.Parse("#F44336")) // Red if negative
                    : new SolidColorBrush(Color.Parse("#FFFFFF")) // White otherwise
            };
            Grid.SetColumn(remainingText, 3);
            rowGrid.Children.Add(remainingText);

            // Column 4: Progress Bar and Percentage
            var progressPanel = new StackPanel
            {
                Spacing = 5,
                Margin = new Thickness(10, 0, 10, 0),
                VerticalAlignment = VerticalAlignment.Center
            };

            var progressBar = new ProgressBar
            {
                Value = budgetItem.Progress,
                Maximum = 100,
                Height = 8,
                CornerRadius = new CornerRadius(4),
                Foreground = new SolidColorBrush(Color.Parse(budgetItem.StatusColor))
            };

            var progressText = new TextBlock
            {
                Text = budgetItem.ProgressText,
                FontSize = 12,
                HorizontalAlignment = HorizontalAlignment.Right
            };

            progressPanel.Children.Add(progressBar);
            progressPanel.Children.Add(progressText);
            Grid.SetColumn(progressPanel, 4);
            rowGrid.Children.Add(progressPanel);

            // Column 5: Status
            var statusBorder = new Border
            {
                Background = new SolidColorBrush(Color.Parse(budgetItem.StatusColor)),
                CornerRadius = new CornerRadius(4),
                Padding = new Thickness(8, 4, 8, 4),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };

            var statusText = new TextBlock
            {
                Text = budgetItem.Status,
                Foreground = new SolidColorBrush(Color.Parse("#FFFFFF")),
                FontSize = 12,
                FontWeight = FontWeight.SemiBold
            };

            statusBorder.Child = statusText;
            Grid.SetColumn(statusBorder, 5);
            rowGrid.Children.Add(statusBorder);

            // Column 6: Action Buttons
            var actionPanel = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Spacing = 5
            };

            // Edit Button
            var editButton = new Button
            {
                Content = "Edit",
                Classes = { "actionButton" },
                Tag = budgetItem.Id,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            editButton.Click += OnEditBudgetClick;

            // Delete Button
            var deleteButton = new Button
            {
                Content = "Delete",
                Classes = { "actionButton", "deleteButton" },
                Tag = budgetItem.Id,
                HorizontalContentAlignment = HorizontalAlignment.Center,
                VerticalContentAlignment = VerticalAlignment.Center,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            deleteButton.Click += OnDeleteBudgetClick;

            actionPanel.Children.Add(editButton);
            actionPanel.Children.Add(deleteButton);
            Grid.SetColumn(actionPanel, 6);
            rowGrid.Children.Add(actionPanel);
        }
        catch (Exception)
        {
            // Error handling
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
            
            // Show the budget creation popup
            var addBudgetPopup = this.Get<Border>("AddBudgetPopup");
            var addBudgetTitle = this.Get<TextBlock>("AddBudgetTitle");
            var addBudgetErrorText = this.Get<TextBlock>("AddBudgetErrorText");
            var budgetNameInput = this.Get<TextBox>("BudgetNameInput");
            var budgetAmountInput = this.Get<TextBox>("BudgetAmountInput");
            var saveBudgetButton = this.Get<Button>("SaveBudgetButton");
            
            if (addBudgetPopup != null)
            {
                // Update popup title
                if (addBudgetTitle != null)
                {
                    addBudgetTitle.Text = "Add New Budget Category";
                }
                
                // Reset the save button tag (no budget ID for new budgets)
                if (saveBudgetButton != null)
                {
                    saveBudgetButton.Tag = null;
                }
                
                // Clear form fields
                if (budgetNameInput != null) budgetNameInput.Text = string.Empty;
                if (budgetAmountInput != null) budgetAmountInput.Text = string.Empty;
                
                // Hide error message
                if (addBudgetErrorText != null)
                {
                    addBudgetErrorText.IsVisible = false;
                }
                
                // Ensure the popup is visible
                addBudgetPopup.IsVisible = true;
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

    private void OnEditBudgetClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (sender is Button button && button.Tag is int budgetId && _userBudgets != null)
            {
                var budget = _userBudgets.FirstOrDefault(b => b.Id == budgetId);
                if (budget != null)
                {
                    
                    // Get references to popup controls
                    var addBudgetPopup = this.Get<Border>("AddBudgetPopup");
                    var addBudgetTitle = this.Get<TextBlock>("AddBudgetTitle");
                    var addBudgetErrorText = this.Get<TextBlock>("AddBudgetErrorText");
                    var budgetNameInput = this.Get<TextBox>("BudgetNameInput");
                    var budgetAmountInput = this.Get<TextBox>("BudgetAmountInput");
                    var saveBudgetButton = this.Get<Button>("SaveBudgetButton");
                    
                    if (addBudgetPopup != null && addBudgetTitle != null && 
                        budgetNameInput != null && budgetAmountInput != null && 
                        saveBudgetButton != null)
                    {
                        // Update popup title
                        addBudgetTitle.Text = "Edit Budget Category";
                        
                        // Set budget data in form fields
                        budgetNameInput.Text = budget.BudgetName;
                        budgetAmountInput.Text = budget.TotalAmountRequired.ToString();
                        
                        // Store the budget ID in the button's Tag for reference when saving
                        saveBudgetButton.Tag = budget.Id;
                        
                        // Hide any error messages
                        if (addBudgetErrorText != null)
                        {
                            addBudgetErrorText.IsVisible = false;
                        }
                        
                        // Show the popup
                        addBudgetPopup.IsVisible = true;
                    }
                }
            }
        }
        catch (Exception)
        {
            // Error handling
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
            
            // Get references to popup controls
            var addBudgetPopup = this.Get<Border>("AddBudgetPopup");
            var addBudgetErrorText = this.Get<TextBlock>("AddBudgetErrorText");
            var budgetNameInput = this.Get<TextBox>("BudgetNameInput");
            var budgetAmountInput = this.Get<TextBox>("BudgetAmountInput");
            var saveBudgetButton = this.Get<Button>("SaveBudgetButton");
            
            // Validate inputs
            if (budgetNameInput == null || budgetAmountInput == null || 
                addBudgetErrorText == null || _budgetController == null || _currentUser == null)
            {
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
            
            // Validate budget name length (max 100 chars)
            if (budgetName.Length > 100)
            {
                addBudgetErrorText.Text = "Budget name cannot exceed 100 characters";
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
            
            // Validate amount max 6 digits
            if (amount.ToString("0").Length > 6)
            {
                addBudgetErrorText.Text = "Amount cannot exceed 6 digits";
                addBudgetErrorText.IsVisible = true;
                return;
            }
            
            // Check if we're updating an existing budget or creating a new one
            bool isUpdating = saveBudgetButton?.Tag is int budgetId && budgetId > 0;
            
            if (isUpdating && saveBudgetButton?.Tag is int editBudgetId)
            {
                // Update existing budget
                var existingBudget = new Budget
                {
                    Id = editBudgetId,
                    UserId = _currentUser.Id,
                    BudgetName = budgetName,
                    TotalAmountRequired = amount
                };
                
                var result = await _budgetController.TryUpdateBudget(existingBudget);
                
                if (result.Success)
                {
                    
                    // Hide the popup
                    if (addBudgetPopup != null)
                    {
                        addBudgetPopup.IsVisible = false;
                    }
                    
                    // Reset the button tag after successful update
                    if (saveBudgetButton != null)
                    {
                        saveBudgetButton.Tag = null;
                    }
                    
                    // Reload budget data
                    await LoadDataAsync();
                }
                else
                {
                    // Get error message from the errors list or use a default message
                    string errorMessage = (result.errors != null && result.errors.Count > 0) 
                        ? string.Join(", ", result.errors) 
                        : "Failed to update budget category";
                        
                    // Show error message
                    addBudgetErrorText.Text = errorMessage;
                    addBudgetErrorText.IsVisible = true;
                }
            }
            else
            {
                // Create new budget
                var newBudget = new Budget
                {
                    UserId = _currentUser.Id,
                    BudgetName = budgetName,
                    TotalAmountRequired = amount
                };
                
                // Add budget to database
                var result = await _budgetController.TryAddBudget(newBudget);
                
                if (result.Success)
                {
                    
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
                }
            }
        }
        catch (Exception ex)
        {
            
            // Show error message in UI
            var addBudgetErrorText = this.Get<TextBlock>("AddBudgetErrorText");
            if (addBudgetErrorText != null)
            {
                addBudgetErrorText.Text = $"An error occurred: {ex.Message}";
                addBudgetErrorText.IsVisible = true;
            }
        }
>>>>>>> Stashed changes
    }

    private void InitializeComponent()
    {
<<<<<<< Updated upstream
        AvaloniaXamlLoader.Load(this);
    }
}
=======
        try
        {
            // Just load the XAML without trying to find controls here
            AvaloniaXamlLoader.Load(this);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading XAML: {ex.Message}");
        }
    }

    private void FindAndConnectControls()
    {
        try
        {

            // Use direct access approach instead of FindControl when possible
            _budgetItemsPanel = this.Get<StackPanel>("BudgetItemsPanel");
            
            _searchBox = this.Get<TextBox>("SearchBox");
            
            _addBudgetPopup = this.Get<Border>("AddBudgetPopup");
            
            _addBudgetErrorText = this.Get<TextBlock>("AddBudgetErrorText");
            
            _addBudgetTitle = this.Get<TextBlock>("AddBudgetTitle");
            
            _saveBudgetButton = this.Get<Button>("SaveBudgetButton");
            
            _budgetNameInput = this.Get<TextBox>("BudgetNameInput");
            
            _budgetAmountInput = this.Get<TextBox>("BudgetAmountInput");
            
            _totalBudgetText = this.Get<TextBlock>("TotalBudgetText");
            
            _remainingBudgetText = this.Get<TextBlock>("RemainingBudgetText");
            
            _statusFilterComboBox = this.Get<ComboBox>("StatusFilterComboBox");
            
            // Connect event handlers - only connect if control is found
            if (_searchBox != null)
            {
                _searchBox.TextChanged -= OnSearchTextChanged; // Remove any existing handler first
                _searchBox.TextChanged += OnSearchTextChanged;
            }
            
            if (_statusFilterComboBox != null)
            {
                _statusFilterComboBox.SelectionChanged -= OnStatusFilterChanged; // Remove any existing handler first
                _statusFilterComboBox.SelectionChanged += OnStatusFilterChanged;
            }
            
            // Ensure the add budget popup is initially hidden
            if (_addBudgetPopup != null)
            {
                _addBudgetPopup.IsVisible = false;
            }
            
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error finding and connecting controls: {ex.Message}");
        }
    }
}
>>>>>>> Stashed changes
