using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace PersonalBudgeting.Views.Pages;

public partial class ExpensesPage : UserControl
{
    public ExpensesPage()
    {
        InitializeComponent();
    }

<<<<<<< Updated upstream
=======
    public ExpensesPage(
        ExpenseController expenseController,
        UserController userController,
        User currentUser)
    {
        InitializeComponent();
        
        _expenseController = expenseController ?? throw new ArgumentNullException(nameof(expenseController));
        _userController = userController ?? throw new ArgumentNullException(nameof(userController));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        
        GetControlReferences();
        
        if (_welcomeMessage != null && _currentUser != null)
        {
            _welcomeMessage.Text = $"Welcome, {_currentUser.UserName}";
        }
        
        LoadExpenseData();
    }
    
    private void GetControlReferences()
    {
        _expenseListPanel = this.FindControl<StackPanel>("ExpenseListPanel");
        _searchBox = this.FindControl<TextBox>("SearchBox");
        _addExpensePopup = this.FindControl<Border>("AddExpensePopup");
        _addExpenseErrorText = this.FindControl<TextBlock>("AddExpenseErrorText");
        _addExpenseTitle = this.FindControl<TextBlock>("AddExpenseTitle");
        _saveExpenseButton = this.FindControl<Button>("SaveExpenseButton");
        _expenseNameInput = this.FindControl<TextBox>("ExpenseNameInput");
        _expenseCategoryComboBox = this.FindControl<ComboBox>("ExpenseCategoryComboBox");
        _requiredAmountInput = this.FindControl<TextBox>("RequiredAmountInput");
        _expenseAmountInput = this.FindControl<TextBox>("ExpenseAmountInput");
        _expenseDatePicker = this.FindControl<DatePicker>("ExpenseDatePicker");
        _reminderDatePicker = this.FindControl<DatePicker>("ReminderDatePicker");
        _welcomeMessage = this.FindControl<TextBlock>("WelcomeMessage");
        _totalExpenseText = this.FindControl<TextBlock>("TotalExpenseText");
        _avgExpenseText = this.FindControl<TextBlock>("AvgExpenseText");
        _expenseCategoriesText = this.FindControl<TextBlock>("ExpenseCategoriesText");
        
        // Add event handler for category selection change
        if (_expenseCategoryComboBox != null)
        {
            _expenseCategoryComboBox.SelectionChanged += OnCategorySelectionChanged;
        }
    }
    
    private async void LoadExpenseData()
    {
        try
        {
            if (_currentUser == null || _currentUser.Id <= 0 || _expenseController == null || _userController == null)
            {
                return;
            }
            
            var result = await _userController.TryGetUserExpenses(_currentUser);
            
            if (result.Success)
            {
                _userExpenses = result.Expenses;
                
                // Update UI
                UpdateSummaryCards();
                UpdateExpenseGrid();
                
                // Load budget categories
                await LoadBudgetCategories();
            }
            else
            {
                _userExpenses = new List<Expense>();
                
                // Still try to load budget categories even if no expenses
                await LoadBudgetCategories();
            }
        }
        catch (Exception ex)
        {
            // Error handling
        }
    }
    
    private void UpdateSummaryCards()
    {
        if (_userExpenses == null)
        {
            return;
        }
        
        // Calculate total expenses
        var totalExpense = _userExpenses.Sum(e => e.SpentAmount);
        if (_totalExpenseText != null)
        {
            _totalExpenseText.Text = totalExpense.ToString("C");
        }
        
        // Calculate average expense
        var avgExpense = _userExpenses.Count > 0 ? totalExpense / _userExpenses.Count : 0;
        if (_avgExpenseText != null)
        {
            _avgExpenseText.Text = avgExpense.ToString("C");
        }
        
        // Count unique categories
        var categoriesCount = _userExpenses
            .Select(e => e.ExpenseName)
            .Distinct()
            .Count();
        
        if (_expenseCategoriesText != null)
        {
            _expenseCategoriesText.Text = categoriesCount.ToString();
        }
    }
    
    private void UpdateExpenseGrid()
    {
        try
        {
            if (_expenseListPanel == null || _userExpenses == null)
            {
                return;
            }
            
            // Clear existing items
            _expenseListPanel.Children.Clear();
            
            // Filter expenses based on search
            var filteredExpenses = _userExpenses
                .Where(e => 
                    string.IsNullOrEmpty(_searchText) || 
                    e.ExpenseName.ToLower().Contains(_searchText.ToLower()))
                .OrderByDescending(e => e.DateCycle)
                .ToList();
            
            // Add null check before dereferencing
            if (filteredExpenses != null)
            {
                foreach (var expense in filteredExpenses)
                {
                    // Create a border for the entire row
                    var rowBorder = new Border
                    {
                        BorderBrush = new SolidColorBrush(Color.Parse("#333333")),
                        BorderThickness = new Thickness(1, 0, 1, 1),
                        Background = new SolidColorBrush(Color.Parse("#2E2E2E")),
                        Height = 50
                    };

                    // Create a grid for this row with 6 columns
                    var rowGrid = new Grid
                    {
                        ColumnDefinitions = new ColumnDefinitions("2*,1*,1*,1*,1*,1*")
                    };
                    
                    // Name column
                    var nameText = new TextBlock
                    {
                        Text = expense.ExpenseName,
                        VerticalAlignment = VerticalAlignment.Center,
                        Margin = new Thickness(10, 0, 0, 0)
                    };
                    Grid.SetColumn(nameText, 0);
                    rowGrid.Children.Add(nameText);
                    
                    // Category column
                    var categoryText = new TextBlock
                    {
                        Text = GetCategoryFromExpenseName(expense.ExpenseName),
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    Grid.SetColumn(categoryText, 1);
                    rowGrid.Children.Add(categoryText);
                    
                    // Required Amount column
                    var requiredText = new TextBlock
                    {
                        Text = expense.RequiredAmount.ToString("C"),
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Foreground = new SolidColorBrush(Color.Parse("#FF9800"))
                    };
                    Grid.SetColumn(requiredText, 2);
                    rowGrid.Children.Add(requiredText);
                    
                    // Spent Amount column
                    var amountText = new TextBlock
                    {
                        Text = expense.SpentAmount.ToString("C"),
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        Foreground = new SolidColorBrush(Color.Parse("#F44336"))
                    };
                    Grid.SetColumn(amountText, 3);
                    rowGrid.Children.Add(amountText);
                    
                    // Date column
                    var dateText = new TextBlock
                    {
                        Text = expense.DateCycle.ToString("yyyy-MM-dd"),
                        VerticalAlignment = VerticalAlignment.Center,
                        HorizontalAlignment = HorizontalAlignment.Center
                    };
                    Grid.SetColumn(dateText, 4);
                    rowGrid.Children.Add(dateText);
                    
                    // Actions column
                    var actionPanel = new StackPanel
                    {
                        Orientation = Orientation.Horizontal,
                        HorizontalAlignment = HorizontalAlignment.Center,
                        VerticalAlignment = VerticalAlignment.Center,
                        Spacing = 5
                    };
                    
                    // Edit button
                    var editButton = new Button
                    {
                        Content = "Edit",
                        Classes = { "actionButton" },
                        Tag = expense.Id,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center
                    };
                    editButton.Click += OnEditExpenseClick;
                    
                    // Delete button
                    var deleteButton = new Button
                    {
                        Content = "Delete",
                        Classes = { "actionButton", "deleteButton" },
                        Tag = expense.Id,
                        HorizontalContentAlignment = HorizontalAlignment.Center,
                        VerticalContentAlignment = VerticalAlignment.Center
                    };
                    deleteButton.Click += OnDeleteExpenseClick;
                    
                    actionPanel.Children.Add(editButton);
                    actionPanel.Children.Add(deleteButton);
                    Grid.SetColumn(actionPanel, 5);
                    rowGrid.Children.Add(actionPanel);
                    
                    // Add the grid to the border
                    rowBorder.Child = rowGrid;
                    
                    // Add the row to the panel
                    _expenseListPanel.Children.Add(rowBorder);
                }
            }
            
            // Show message if no expenses found
            if (filteredExpenses?.Count == 0)
            {
                var noExpensesText = new TextBlock
                {
                    Text = "No expenses found. Add your first expense!",
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Margin = new Thickness(0, 20, 0, 0)
                };
                _expenseListPanel?.Children.Add(noExpensesText);
            }
        }
        catch (Exception ex)
        {
            // Error handling
        }
    }
    
    private string GetCategoryFromExpenseName(string expenseName)
    {
        // Extract the category part from the expense name format "Category - Name"
        var parts = expenseName.Split(new[] { " - " }, 2, StringSplitOptions.None);
        
        if (parts.Length == 2)
        {
            return parts[0]; // Return the category part
        }
        
        // If not in expected format, return the whole name
        return expenseName;
    }
    
    private async void OnAddExpenseClick(object? sender, RoutedEventArgs e)
    {
        _isEditing = false;
        
        if (_addExpensePopup != null && _addExpenseTitle != null && _saveExpenseButton != null)
        {
            _addExpenseTitle.Text = "Add New Expense";
            _saveExpenseButton.Tag = null;
            
            // Reload budget categories each time the popup is opened
            await LoadBudgetCategories();
            
            // Clear form fields
            if (_expenseNameInput != null) _expenseNameInput.Text = string.Empty;
            if (_requiredAmountInput != null) _requiredAmountInput.Text = string.Empty;
            if (_expenseAmountInput != null) _expenseAmountInput.Text = string.Empty;
            if (_expenseDatePicker != null) _expenseDatePicker.SelectedDate = DateTime.Today;
            
            // Explicitly set reminder date picker to null (optional field)
            if (_reminderDatePicker != null) 
            {
                _reminderDatePicker.SelectedDate = null;
            }
            
            // Hide error message
            if (_addExpenseErrorText != null)
            {
                _addExpenseErrorText.IsVisible = false;
            }
            
            _addExpensePopup.IsVisible = true;
        }
    }
    
    private void OnCancelAddExpenseClick(object? sender, RoutedEventArgs e)
    {
        if (_addExpensePopup != null)
        {
            _addExpensePopup.IsVisible = false;
        }
    }
    
    private async void OnSaveExpenseClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            if (_expenseController == null || _currentUser == null || 
                _expenseNameInput == null || _expenseCategoryComboBox == null ||
                _requiredAmountInput == null || _expenseAmountInput == null || 
                _expenseDatePicker == null || _addExpenseErrorText == null || 
                _saveExpenseButton == null || _reminderDatePicker == null)
            {
                return;
            }
            
            // Validate inputs
            if (string.IsNullOrWhiteSpace(_expenseNameInput.Text))
            {
                _addExpenseErrorText.Text = "Please enter an expense name";
                _addExpenseErrorText.IsVisible = true;
                return;
            }
            
            // Validate expense name length (max 100 chars)
            if (_expenseNameInput.Text.Length > 100)
            {
                _addExpenseErrorText.Text = "Expense name cannot exceed 100 characters";
                _addExpenseErrorText.IsVisible = true;
                return;
            }
            
            // Get selected category
            if (_expenseCategoryComboBox.SelectedItem == null)
            {
                _addExpenseErrorText.Text = "Please select a category";
                _addExpenseErrorText.IsVisible = true;
                return;
            }
            
            var selectedItem = _expenseCategoryComboBox.SelectedItem as ComboBoxItem;
            if (selectedItem == null)
            {
                return;
            }
            
            var selectedCategory = selectedItem.Content?.ToString();
            if (string.IsNullOrEmpty(selectedCategory))
            {
                return;
            }
            
            // Validate required amount
            if (string.IsNullOrWhiteSpace(_requiredAmountInput.Text) || 
                !decimal.TryParse(_requiredAmountInput.Text, out var requiredAmount) || 
                requiredAmount <= 0)
            {
                _addExpenseErrorText.Text = "Please enter a valid required amount greater than zero";
                _addExpenseErrorText.IsVisible = true;
                return;
            }
            
            // Validate required amount max 6 digits
            if (requiredAmount.ToString("0").Length > 6)
            {
                _addExpenseErrorText.Text = "Required amount cannot exceed 6 digits";
                _addExpenseErrorText.IsVisible = true;
                return;
            }
            
            // Validate spent amount
            if (string.IsNullOrWhiteSpace(_expenseAmountInput.Text) || 
                !decimal.TryParse(_expenseAmountInput.Text, out var spentAmount) || 
                spentAmount < 0)
            {
                _addExpenseErrorText.Text = "Please enter a valid spent amount (must be 0 or greater)";
                _addExpenseErrorText.IsVisible = true;
                return;
            }
            
            // Validate spent amount max 6 digits
            if (spentAmount.ToString("0").Length > 6)
            {
                _addExpenseErrorText.Text = "Spent amount cannot exceed 6 digits";
                _addExpenseErrorText.IsVisible = true;
                return;
            }
            
            // Validate date
            if (_expenseDatePicker.SelectedDate == null)
            {
                _addExpenseErrorText.Text = "Please select a date";
                _addExpenseErrorText.IsVisible = true;
                return;
            }
            
            // Get budget ID for this category (optional, will use default if not found)
            int budgetId = DEFAULT_BUDGET_ID;
            try
            {
                // First check if we're editing and already have a budgetId
                if (_isEditing && _saveExpenseButton.Tag != null)
                {
                    var expenseId = (int)_saveExpenseButton.Tag;
                    var existingExpense = _userExpenses?.FirstOrDefault(e => e.Id == expenseId);
                    if (existingExpense != null)
                    {
                        budgetId = existingExpense.BudgetId;
                    }
                }
                else
                {
                    // Try to find a matching budget for this category
                    if (_userController != null)
                    {
                        var budgetsResult = await _userController.TryGetUserBudgets(_currentUser);
                        if (budgetsResult.Success)
                        {
                            var budgets = budgetsResult.Budgets;
                            if (budgets != null && budgets.Count > 0)
                            {
                                // Find the budget for the selected category
                                var matchingBudget = budgets.FirstOrDefault(b => 
                                    b.BudgetName.StartsWith(selectedCategory + " - ", StringComparison.OrdinalIgnoreCase) || 
                                    b.BudgetName.Contains(selectedCategory, StringComparison.OrdinalIgnoreCase));
                                    
                                if (matchingBudget != null)
                                {
                                    budgetId = matchingBudget.Id;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Error handling
            }
            
            // Combine category with expense name
            string expenseName = string.IsNullOrEmpty(_expenseNameInput.Text.Trim()) 
                ? selectedCategory 
                : $"{selectedCategory} - {_expenseNameInput.Text.Trim()}";
            
            // Parse reminder date if set (optional)
            DateTime? reminderTime = null;
            if (_reminderDatePicker.SelectedDate.HasValue)
            {
                reminderTime = _reminderDatePicker.SelectedDate.Value.DateTime;
            }
            
            // Create expense object with all required fields
            var expense = new Expense
            {
                UserId = _currentUser.Id,
                ExpenseName = expenseName,
                RequiredAmount = requiredAmount,
                SpentAmount = spentAmount,
                BudgetId = budgetId,
                DateCycle = _expenseDatePicker.SelectedDate.Value.DateTime,
                // Only set ReminderTime if we successfully processed it
                ReminderTime = reminderTime 
            };
            
            Console.WriteLine($"Saving expense: {expense}");
            
            // Check if we're editing or adding
            var isEditing = _isEditing && _saveExpenseButton.Tag != null ? (int)_saveExpenseButton.Tag : 0;
            
            if (isEditing > 0)
            {
                // Update existing expense
                expense.Id = isEditing;
                var result = await _expenseController.TryUpdateExpense(expense);
                
                if (result.Success)
                {
                    // Update expense in the local list
                    if (_userExpenses != null)
                    {
                        var index = _userExpenses.FindIndex(i => i.Id == expense.Id);
                        if (index >= 0)
                        {
                            _userExpenses[index] = expense;
                        }
                    }
                    
                    // Hide popup
                    if (_addExpensePopup != null)
                    {
                        _addExpensePopup.IsVisible = false;
                    }
                    
                    // Update UI
                    UpdateSummaryCards();
                    UpdateExpenseGrid();
                }
                else
                {
                    if (_addExpenseErrorText != null)
                    {
                        string errorMessage = result.Errors != null && result.Errors.Count > 0
                            ? string.Join(", ", result.Errors)
                            : "Failed to update expense";
                            
                        _addExpenseErrorText.Text = errorMessage;
                        _addExpenseErrorText.IsVisible = true;
                    }
                }
            }
            else
            {
                // Add new expense
                var result = await _expenseController.TryAddExpense(expense);
                
                if (result.Success)
                {
                    // Add expense to the local list
                    if (_userExpenses != null)
                    {
                        // Set the ID from the result
                        expense.Id = result.ExpenseId;
                        _userExpenses.Add(expense);
                    }
                    
                    // Hide popup
                    if (_addExpensePopup != null)
                    {
                        _addExpensePopup.IsVisible = false;
                    }
                    
                    // Update UI
                    UpdateSummaryCards();
                    UpdateExpenseGrid();
                }
                else
                {
                    if (_addExpenseErrorText != null)
                    {
                        string errorMessage = result.Errors != null && result.Errors.Count > 0
                            ? string.Join(", ", result.Errors)
                            : "Failed to add expense";
                            
                        _addExpenseErrorText.Text = errorMessage;
                        _addExpenseErrorText.IsVisible = true;
                    }
                }
            }
        }
        catch (Exception ex)
        {
            if (_addExpenseErrorText != null)
            {
                _addExpenseErrorText.Text = $"An error occurred: {ex.Message}";
                _addExpenseErrorText.IsVisible = true;
            }
        }
    }
    
    private async void OnEditExpenseClick(object? sender, RoutedEventArgs e)
    {
        if (_userExpenses == null || !(sender is Button button) || button.Tag == null ||
            _addExpensePopup == null || _addExpenseTitle == null || _saveExpenseButton == null ||
            _expenseNameInput == null || _expenseCategoryComboBox == null ||
            _requiredAmountInput == null || _expenseAmountInput == null ||
            _expenseDatePicker == null || _reminderDatePicker == null || _addExpenseErrorText == null)
        {
            return;
        }
        
        // Get expense ID from button tag
        var expenseId = (int)button.Tag;
        var expense = _userExpenses.FirstOrDefault(i => i.Id == expenseId);
        
        if (expense == null)
        {
            return;
        }
        
        _isEditing = true;
        _addExpenseTitle.Text = "Edit Expense";
        _saveExpenseButton.Tag = expenseId;
        
        // Reload budget categories to ensure they're up to date
        await LoadBudgetCategories();
        
        // Extract category and actual expense name from the combined name
        var (category, name) = ExtractCategoryAndName(expense.ExpenseName);
        
        // Set fields with expense values
        _expenseNameInput.Text = name;
        
        // Find the matching category in the combobox
        int categoryIndex = -1;
        for (int i = 0; i < _expenseCategoryComboBox.Items.Count; i++)
        {
            if (_expenseCategoryComboBox.Items[i] is ComboBoxItem item && 
                item.Content?.ToString() == category)
            {
                categoryIndex = i;
                break;
            }
        }
        
        // Set the category dropdown
        _expenseCategoryComboBox.SelectedIndex = categoryIndex >= 0 ? categoryIndex : 0;
        
        _requiredAmountInput.Text = expense.RequiredAmount.ToString("0.00");
        _expenseAmountInput.Text = expense.SpentAmount.ToString("0.00");
        _expenseDatePicker.SelectedDate = expense.DateCycle;
        
        // Set reminder date picker, handling null case properly
        if (expense.ReminderTime.HasValue)
        {
            try
            {
                _reminderDatePicker.SelectedDate = expense.ReminderTime.Value.Date;
            }
            catch (Exception)
            {
                _reminderDatePicker.SelectedDate = null;
            }
        }
        else
        {
            _reminderDatePicker.SelectedDate = null;
        }
        
        _addExpenseErrorText.IsVisible = false;
        
        // Show popup
        _addExpensePopup.IsVisible = true;
    }
    
    private (string Category, string Name) ExtractCategoryAndName(string expenseName)
    {
        // Try to extract category from format "Category - Name"
        var parts = expenseName.Split(new[] { " - " }, 2, StringSplitOptions.None);
        
        if (parts.Length == 2)
        {
            return (parts[0], parts[1]);
        }
        
        // If not in expected format, return the whole string as both parts
        return (expenseName, expenseName);
    }
    
    private async void OnDeleteExpenseClick(object? sender, RoutedEventArgs e)
    {
        if (_userExpenses == null || _expenseController == null || !(sender is Button button) || button.Tag == null)
        {
            return;
        }
        
        // Get expense ID from button tag
        var expenseId = (int)button.Tag;
        
        // Find the expense in our list
        var expense = _userExpenses.FirstOrDefault(i => i.Id == expenseId);
        if (expense == null)
        {
            return;
        }
        
        // Delete from database
        var result = await _expenseController.TryDeleteExpense(expense);
        
        if (result.Success)
        {
            // Remove from local list
            _userExpenses.RemoveAll(i => i.Id == expenseId);
            
            // Update UI
            UpdateSummaryCards();
            UpdateExpenseGrid();
        }
    }
    
    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        if (sender is TextBox textBox)
        {
            _searchText = textBox.Text ?? string.Empty;
            UpdateExpenseGrid();
        }
    }
    
    private async void OnCategorySelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (_requiredAmountInput == null || _expenseCategoryComboBox == null || 
            _expenseCategoryComboBox.SelectedItem == null || _userController == null || 
            _currentUser == null)
        {
            return;
        }
        
        var selectedItem = _expenseCategoryComboBox.SelectedItem as ComboBoxItem;
        if (selectedItem == null)
        {
            return;
        }
        
        var selectedCategory = selectedItem.Content?.ToString();
        if (string.IsNullOrEmpty(selectedCategory))
        {
            return;
        }
        
        try
        {
            // Get all budgets for the user
            var budgetsResult = await _userController.TryGetUserBudgets(_currentUser);
            if (!budgetsResult.Success)
            {
                if (budgetsResult.errors != null)
                {
                    // Handle error
                }
                return;
            }
            
            var budgets = budgetsResult.Budgets;
            if (budgets == null || budgets.Count == 0)
            {
                return;
            }
            
            // Find the budget with the exact name match (since we're now using budget names directly as categories)
            var matchingBudget = budgets.FirstOrDefault(b => 
                string.Equals(b.BudgetName, selectedCategory, StringComparison.OrdinalIgnoreCase));
            
            if (matchingBudget != null)
            {
                // Set the required amount to the budget amount
                _requiredAmountInput.Text = matchingBudget.TotalAmountRequired.ToString("0.00");
                
                // Store the budget ID for use when saving
                if (_saveExpenseButton != null)
                {
                    // Remove any existing budget ID classes
                    var classesToRemove = _saveExpenseButton.Classes
                        .Where(c => c.StartsWith("budgetId-"))
                        .ToList();
                    
                    foreach (var cls in classesToRemove)
                    {
                        _saveExpenseButton.Classes.Remove(cls);
                    }
                    
                    // Add the new budget ID
                    _saveExpenseButton.Classes.Add("budgetId-" + matchingBudget.Id);
                }
            }
            else
            {
                // Clear the required amount or set to default
                _requiredAmountInput.Text = "";
                
                // Clear any stored budget ID
                if (_saveExpenseButton != null)
                {
                    var classesToRemove = _saveExpenseButton.Classes
                        .Where(c => c.StartsWith("budgetId-"))
                        .ToList();
                    
                    foreach (var cls in classesToRemove)
                    {
                        _saveExpenseButton.Classes.Remove(cls);
                    }
                }
            }
        }
        catch (Exception)
        {
            // Error handling
        }
    }
    
    private async Task LoadBudgetCategories()
    {
        try
        {
            if (_expenseCategoryComboBox == null || _userController == null || _currentUser == null)
            {
                return;
            }
            
            // Clear existing items
            _expenseCategoryComboBox.Items.Clear();
            
            // Get all budgets for the user
            var budgetsResult = await _userController.TryGetUserBudgets(_currentUser);
            if (!budgetsResult.Success)
            {
                if (budgetsResult.errors != null)
                {
                    // Handle error
                }
                // Add default "Other" category
                _expenseCategoryComboBox.Items.Add(new ComboBoxItem { Content = "Other" });
                return;
            }
            
            var budgets = budgetsResult.Budgets;
            if (budgets == null || budgets.Count == 0)
            {
                // Add default "Other" category
                _expenseCategoryComboBox.Items.Add(new ComboBoxItem { Content = "Other" });
                return;
            }
            
            // Add each budget name directly as a category
            foreach (var budget in budgets)
            {
                if (!string.IsNullOrEmpty(budget.BudgetName))
                {
                    _expenseCategoryComboBox.Items.Add(new ComboBoxItem { Content = budget.BudgetName });
                }
            }
            
            // If no categories found, add default "Other"
            if (_expenseCategoryComboBox.Items.Count == 0)
            {
                _expenseCategoryComboBox.Items.Add(new ComboBoxItem { Content = "Other" });
            }
            
            // Select the first item
            _expenseCategoryComboBox.SelectedIndex = 0;
        }
        catch (Exception)
        {
            // Add default "Other" category
            if (_expenseCategoryComboBox != null)
            {
                _expenseCategoryComboBox.Items.Clear();
                _expenseCategoryComboBox.Items.Add(new ComboBoxItem { Content = "Other" });
                _expenseCategoryComboBox.SelectedIndex = 0;
            }
        }
    }
    
>>>>>>> Stashed changes
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}