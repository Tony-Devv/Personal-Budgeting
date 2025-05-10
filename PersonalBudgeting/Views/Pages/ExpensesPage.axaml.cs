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
using Avalonia.VisualTree;
using Controller;
using Model.Entities;
using PersonalBudgeting.Views.Pages;

namespace PersonalBudgeting.Views.Pages;

public partial class ExpensesPage : UserControl
{
    private readonly ExpenseController _expenseController;
    private readonly UserController _userController;
    private readonly BudgetController _budgetController;
    private List<Expense> _userExpenses;
    private User _currentUser;
    
    // Pagination and filtering
    private int _currentPage = 1;
    private int _totalPages = 1;
    private const int _itemsPerPage = 10;
    private string _searchText = string.Empty;

    // ViewModel for expense items in the grid
    public class ExpenseViewModel
    {
        public int Id { get; set; }
        public string Date { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public string Amount { get; set; } = string.Empty;
    }

    public ExpensesPage()
    {
        InitializeComponent();
        
        // Initialize controllers
        _expenseController = new ExpenseController();
        _userController = new UserController();
        _budgetController = new BudgetController();
        
        // In a real app, this would be from authentication
        _currentUser = new User { Id = 1, UserName = "testuser" };

        // Set up event handlers
        var searchBox = this.FindControl<TextBox>("SearchBox");
        var prevPageButton = this.FindControl<Button>("PrevPageButton");
        var nextPageButton = this.FindControl<Button>("NextPageButton");
            
        if (searchBox != null)
            searchBox.TextChanged += OnSearchTextChanged;
            
        if (prevPageButton != null)
            prevPageButton.Click += OnPrevPageClick;
            
        if (nextPageButton != null)
            nextPageButton.Click += OnNextPageClick;

        // Create default user expenses
        _userExpenses = new List<Expense>
        {
            new Expense { Id = 1, UserId = _currentUser.Id, ExpenseName = "Groceries", RequiredAmount = 150.00M, SpentAmount = 120.00M, DateCycle = DateTime.Now.AddDays(-5), BudgetId = 1 },
            new Expense { Id = 2, UserId = _currentUser.Id, ExpenseName = "Utilities", RequiredAmount = 200.00M, SpentAmount = 180.00M, DateCycle = DateTime.Now.AddDays(-3), BudgetId = 2 },
            new Expense { Id = 3, UserId = _currentUser.Id, ExpenseName = "Entertainment", RequiredAmount = 100.00M, SpentAmount = 75.00M, DateCycle = DateTime.Now.AddDays(-1), BudgetId = 3 },
            new Expense { Id = 4, UserId = _currentUser.Id, ExpenseName = "Transportation", RequiredAmount = 120.00M, SpentAmount = 110.00M, DateCycle = DateTime.Now.AddDays(-10), BudgetId = 4 },
            new Expense { Id = 5, UserId = _currentUser.Id, ExpenseName = "Dining Out", RequiredAmount = 180.00M, SpentAmount = 150.00M, DateCycle = DateTime.Now.AddDays(-7), BudgetId = 5 }
        };

        // Load data
        LoadData();
    }

    public ExpensesPage(
        ExpenseController expenseController,
        UserController userController,
        User currentUser,
        List<Expense> userExpenses)
    {
        InitializeComponent();
        
        // Set controllers and user data
        _expenseController = expenseController;
        _userController = userController;
        _budgetController = new BudgetController();
        _userExpenses = userExpenses ?? new List<Expense>();
        _currentUser = currentUser;
        
        // Set up event handlers
        var searchBox = this.FindControl<TextBox>("SearchBox");
        var prevPageButton = this.FindControl<Button>("PrevPageButton");
        var nextPageButton = this.FindControl<Button>("NextPageButton");
            
        if (searchBox != null)
            searchBox.TextChanged += OnSearchTextChanged;
            
        if (prevPageButton != null)
            prevPageButton.Click += OnPrevPageClick;
            
        if (nextPageButton != null)
            nextPageButton.Click += OnNextPageClick;
        
        // Load data
        LoadData();
    }
    
    private void LoadData()
    {
        try
        {
            Console.WriteLine("Loading expense data for user: " + _currentUser.Id);
            
            // Update the summary cards
            UpdateSummaryCards();
            
            // Update expense grid
            UpdateExpenseGrid();
            
            // Update the chart
            UpdateExpenseChart();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading data: {ex.Message}");
        }
    }
    
    private void UpdateSummaryCards()
    {
        // Calculate total expenses
        var totalExpenses = (double)_userExpenses.Sum(e => e.RequiredAmount);
        var totalExpensesText = this.FindControl<TextBlock>("TotalExpensesText");
        if (totalExpensesText != null)
        {
            totalExpensesText.Text = totalExpenses.ToString("C");
        }

        // Calculate expense change (mock data for demonstration)
        var previousTotalExpenses = totalExpenses > 0 ? totalExpenses * 0.9 : 0; // Mock data
        var expenseChange = totalExpenses > 0 && previousTotalExpenses > 0
            ? ((totalExpenses - previousTotalExpenses) / previousTotalExpenses) * 100
            : 0;
        
        var expenseChangeText = this.FindControl<TextBlock>("ExpenseChangeText");
        if (expenseChangeText != null)
        {
            var changePrefix = expenseChange > 0 ? "+" : "";
            expenseChangeText.Text = $"{changePrefix}{expenseChange:F1}%";
        }

        // Calculate average expense
        var averageExpense = _userExpenses.Count > 0 
            ? (double)_userExpenses.Average(e => e.RequiredAmount) 
            : 0;
        
        var averageExpenseText = this.FindControl<TextBlock>("AverageExpenseText");
        if (averageExpenseText != null)
        {
            averageExpenseText.Text = averageExpense.ToString("C");
        }

        // Calculate top category
        var topCategory = _userExpenses
            .GroupBy(e => e.ExpenseName)
            .OrderByDescending(g => g.Sum(e => e.RequiredAmount))
            .FirstOrDefault();
        
        var topCategoryText = this.FindControl<TextBlock>("TopCategoryText");
        if (topCategoryText != null)
        {
            topCategoryText.Text = topCategory?.Key ?? "None";
        }
        
        var topCategoryPercentText = this.FindControl<TextBlock>("TopCategoryPercentText");
        if (topCategoryPercentText != null && topCategory != null && totalExpenses > 0)
        {
            var categoryTotal = (double)topCategory.Sum(e => e.RequiredAmount);
            var percentage = (categoryTotal / totalExpenses) * 100;
            topCategoryPercentText.Text = $"{percentage:F1}% of expenses";
        }

        // Set budget status based on whether we're over budget
        var budgetStatusText = this.FindControl<TextBlock>("BudgetStatusText");
        if (budgetStatusText != null)
        {
            // Mock budget status - in a real app, you'd calculate this based on actual budget data
            var isOverBudget = totalExpenses > 5000; // Sample threshold
            budgetStatusText.Text = isOverBudget ? "Over Budget" : "On Track";
        }
    }
    
    private void UpdateExpenseGrid()
    {
        try
        {
            if (_userExpenses == null) return;
            
            var expenseListPanel = this.FindControl<Panel>("ExpenseListPanel");
            var pageInfoText = this.FindControl<TextBlock>("PageInfoText");
            var prevPageButton = this.FindControl<Button>("PrevPageButton");
            var nextPageButton = this.FindControl<Button>("NextPageButton");
            
            if (expenseListPanel == null || pageInfoText == null || prevPageButton == null || nextPageButton == null)
                return;
            
            // Clear existing items
            expenseListPanel.Children.Clear();
            
            // Get search text
            var searchText = _searchText.ToLower();
            
            // Filter expenses based on search text
            var filteredExpenses = _userExpenses
                .Where(e => 
                    (string.IsNullOrEmpty(searchText) || 
                    e.ExpenseName.ToLower().Contains(searchText)))
                .OrderByDescending(e => e.DateCycle)
                .ToList();
            
            // Calculate pagination
            _totalPages = (int)Math.Ceiling(filteredExpenses.Count / (double)_itemsPerPage);
            _totalPages = Math.Max(1, _totalPages); // At least 1 page
            _currentPage = Math.Min(_currentPage, _totalPages);
            
            // Update current page text
            pageInfoText.Text = $"Page {_currentPage} of {_totalPages}";
            
            // Enable/disable pagination buttons
            prevPageButton.IsEnabled = _currentPage > 1;
            nextPageButton.IsEnabled = _currentPage < _totalPages;
            
            // Get current page items
            var pageItems = filteredExpenses
                .Skip((_currentPage - 1) * _itemsPerPage)
                .Take(_itemsPerPage)
                .ToList();
            
            // Create UI for each expense
            foreach (var expense in pageItems)
            {
                var expensePanel = new Border
                {
                    Background = new SolidColorBrush(Color.Parse("#2A2A2A")),
                    CornerRadius = new CornerRadius(4),
                    Padding = new Thickness(10),
                    Margin = new Thickness(0, 0, 0, 5)
                };
                
                var expenseGrid = new Grid();
                expenseGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });
                expenseGrid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
                
                // Left section - Expense info
                var infoSection = new StackPanel { Spacing = 5 };
                
                // Date and name
                var titlePanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 10 };
                
                var dateText = new TextBlock
                {
                    Text = expense.DateCycle.ToString("yyyy-MM-dd"),
                    FontWeight = FontWeight.SemiBold,
                    Opacity = 0.7
                };
                
                var nameText = new TextBlock
                {
                    Text = expense.ExpenseName,
                    FontWeight = FontWeight.Bold,
                    FontSize = 16
                };
                
                titlePanel.Children.Add(dateText);
                titlePanel.Children.Add(nameText);
                
                // Amount
                var amountPanel = new StackPanel { Orientation = Orientation.Horizontal, Spacing = 5 };
                
                var requiredText = new TextBlock
                {
                    Text = $"Required: ${expense.RequiredAmount:N2}",
                    Foreground = new SolidColorBrush(Color.Parse("#dc3545"))
                };
                
                var spentText = new TextBlock
                {
                    Text = $" | Spent: ${expense.SpentAmount:N2}",
                    Foreground = new SolidColorBrush(expense.SpentAmount <= expense.RequiredAmount ? 
                        Color.Parse("#28a745") : Color.Parse("#dc3545"))
                };
                
                amountPanel.Children.Add(requiredText);
                amountPanel.Children.Add(spentText);
                
                infoSection.Children.Add(titlePanel);
                infoSection.Children.Add(amountPanel);
                
                // Right section - Buttons
                var buttonSection = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 5,
                    VerticalAlignment = VerticalAlignment.Center,
                    HorizontalAlignment = HorizontalAlignment.Right
                };
                
                var editButton = new Button
                {
                    Content = "Edit",
                    Tag = expense.Id.ToString()
                };
                editButton.Click += OnEditExpenseClick;
                
                var deleteButton = new Button
                {
                    Content = "Delete",
                    Tag = expense.Id.ToString(),
                    Foreground = new SolidColorBrush(Colors.White),
                    Background = new SolidColorBrush(Color.Parse("#dc3545"))
                };
                deleteButton.Click += OnDeleteExpenseClick;
                
                buttonSection.Children.Add(editButton);
                buttonSection.Children.Add(deleteButton);
                
                // Add sections to grid
                Grid.SetColumn(infoSection, 0);
                Grid.SetColumn(buttonSection, 1);
                
                expenseGrid.Children.Add(infoSection);
                expenseGrid.Children.Add(buttonSection);
                
                expensePanel.Child = expenseGrid;
                expenseListPanel.Children.Add(expensePanel);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating expense grid: {ex.Message}");
        }
    }

    private void UpdateExpenseChart()
    {
        var chartContainer = this.FindControl<Grid>("ChartContainer");
        if (chartContainer == null) return;
        
        // Clear existing chart
        chartContainer.Children.Clear();
        
        try
        {
            // Create a pie chart showing expenses by category
            var categoryExpenses = _userExpenses
                .GroupBy(e => e.ExpenseName)
                .Select(g => new
                {
                    Category = g.Key,
                    Amount = (double)g.Sum(e => e.RequiredAmount)
                })
                .OrderByDescending(item => item.Amount)
                .ToList();
            
            if (categoryExpenses.Count == 0) return;
            
            // Create a simple pie chart (this is a very basic example)
            var pieChart = new Grid
            {
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                Width = 300,
                Height = 300
            };
            
            // In a real app, you would use a charting library
            // For now, we'll create a simple legend instead
            var legend = new StackPanel
            {
                Orientation = Orientation.Vertical,
                Spacing = 5,
                Margin = new Thickness(10)
            };
            
            // Add legend items
            var total = categoryExpenses.Sum(c => c.Amount);
            var colorIndex = 0;
            var colors = new[] 
            { 
                new SolidColorBrush(Colors.Blue),
                new SolidColorBrush(Colors.Green),
                new SolidColorBrush(Colors.Red),
                new SolidColorBrush(Colors.Orange),
                new SolidColorBrush(Colors.Purple)
            };
            
            foreach (var category in categoryExpenses)
            {
                var percentage = (category.Amount / total) * 100;
                var legendItem = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Spacing = 5
                };
                
                var colorBox = new Border
                {
                    Width = 15,
                    Height = 15,
                    Background = colors[colorIndex % colors.Length],
                    CornerRadius = new CornerRadius(2)
                };
                
                var labelText = new TextBlock
                {
                    Text = $"{category.Category}: {category.Amount:C} ({percentage:F1}%)",
                    VerticalAlignment = VerticalAlignment.Center
                };
                
                legendItem.Children.Add(colorBox);
                legendItem.Children.Add(labelText);
                legend.Children.Add(legendItem);
                
                colorIndex++;
            }
            
            // Add the legend to the chart container
            chartContainer.Children.Add(legend);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating chart: {ex.Message}");
        }
    }

    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        var searchBox = sender as TextBox;
        if (searchBox == null) return;
        
        _searchText = searchBox.Text ?? string.Empty;
        _currentPage = 1;
        UpdateExpenseGrid();
    }
    
    private void OnChartPeriodChanged(object? sender, SelectionChangedEventArgs e)
    {
        // Simple console log for now
        Console.WriteLine("Chart period changed");
    }

    private void OnAddExpenseClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var popup = this.FindControl<Border>("AddExpensePopup");
            if (popup != null)
            {
                // Reset inputs
                var nameInput = this.FindControl<TextBox>("ExpenseNameInput");
                var requiredAmountInput = this.FindControl<TextBox>("RequiredAmountInput");
                var spentAmountInput = this.FindControl<TextBox>("SpentAmountInput");
                var dateCyclePicker = this.FindControl<DatePicker>("DateCyclePicker");
                var reminderTimePicker = this.FindControl<DatePicker>("ReminderTimePicker");
                var categoryComboBox = this.FindControl<ComboBox>("ExpenseCategoryComboBox");
                var errorText = this.FindControl<TextBlock>("AddExpenseErrorText");
                
                if (nameInput != null)
                    nameInput.Text = string.Empty;
                
                if (requiredAmountInput != null)
                    requiredAmountInput.Text = string.Empty;
                
                if (spentAmountInput != null)
                    spentAmountInput.Text = string.Empty;
                
                if (dateCyclePicker != null)
                    dateCyclePicker.SelectedDate = DateTime.Today;
                
                if (reminderTimePicker != null)
                    reminderTimePicker.SelectedDate = null;
                
                if (categoryComboBox != null)
                    categoryComboBox.SelectedIndex = -1;
                
                if (errorText != null)
                    errorText.IsVisible = false;
                
                // Show popup
                popup.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error showing expense popup: {ex.Message}");
        }
    }
    
    private void OnCancelAddExpenseClick(object? sender, RoutedEventArgs e)
    {
        var popup = this.FindControl<Border>("AddExpensePopup");
        if (popup != null)
        {
            popup.IsVisible = false;
        }
    }
    
    private void OnSaveExpenseClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            var nameInput = this.FindControl<TextBox>("ExpenseNameInput");
            var requiredAmountInput = this.FindControl<TextBox>("RequiredAmountInput");
            var spentAmountInput = this.FindControl<TextBox>("SpentAmountInput");
            var dateCyclePicker = this.FindControl<DatePicker>("DateCyclePicker");
            var reminderTimePicker = this.FindControl<DatePicker>("ReminderTimePicker");
            var categoryComboBox = this.FindControl<ComboBox>("ExpenseCategoryComboBox");
            var errorText = this.FindControl<TextBlock>("AddExpenseErrorText");
            var popup = this.FindControl<Border>("AddExpensePopup");
            
            if (nameInput == null || requiredAmountInput == null || spentAmountInput == null || 
                dateCyclePicker == null || errorText == null || popup == null)
                return;
            
            // Validate input
            var name = nameInput.Text;
            if (string.IsNullOrWhiteSpace(name))
            {
                errorText.Text = "Expense name is required";
                errorText.IsVisible = true;
                return;
            }
            
            if (!decimal.TryParse(requiredAmountInput.Text, out decimal requiredAmount) || requiredAmount <= 0)
            {
                errorText.Text = "Please enter a valid required amount";
                errorText.IsVisible = true;
                return;
            }
            
            if (!decimal.TryParse(spentAmountInput.Text, out decimal spentAmount) || spentAmount < 0)
            {
                errorText.Text = "Please enter a valid spent amount";
                errorText.IsVisible = true;
                return;
            }
            
            var dateCycle = dateCyclePicker.SelectedDate ?? DateTimeOffset.Now;
            var reminderTime = reminderTimePicker?.SelectedDate;
            
            string category = "Other";
            if (categoryComboBox != null && categoryComboBox.SelectedItem is ComboBoxItem selectedItem)
            {
                category = selectedItem.Content?.ToString() ?? "Other";
            }
            
            // Create expense object
            var expense = new Expense
            {
                UserId = _currentUser.Id,
                ExpenseName = name,
                RequiredAmount = requiredAmount,
                SpentAmount = spentAmount,
                DateCycle = dateCycle.DateTime,
                ReminderTime = reminderTime?.DateTime,
                BudgetId = 1  // Default budget ID, in a real app you would select the correct budget
            };
            
            // In a real app, this would save to database using controller
            // For now, we'll just add it to our local list
            expense.Id = _userExpenses.Any() ? _userExpenses.Max(e => e.Id) + 1 : 1;
            _userExpenses.Add(expense);
            
            // Update UI
            UpdateSummaryCards();
            UpdateExpenseGrid();
            
            // Hide popup
            popup.IsVisible = false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving expense: {ex.Message}");
        }
    }

    private void OnEditExpenseClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag != null)
        {
            // Convert the Tag to an integer ID
            if (int.TryParse(button.Tag.ToString(), out int expenseId))
            {
                var expense = _userExpenses.FirstOrDefault(ex => ex.Id == expenseId);
                if (expense != null)
                {
                    // For now, just log the action
                    Console.WriteLine($"Edit expense request: {expense.ExpenseName} ({expense.RequiredAmount:C})");
                    
                    // Here you would typically show a dialog or navigate to an edit page
                    // For demo purposes, let's just modify the expense
                    expense.ExpenseName = expense.ExpenseName + " (Edited)";
                    expense.RequiredAmount = expense.RequiredAmount * 1.1M; // Increase by 10%
                    
                    // Refresh the grid
                    LoadData();
                }
            }
        }
    }

    private void OnDeleteExpenseClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag != null)
        {
            // Convert the Tag to an integer ID
            if (int.TryParse(button.Tag.ToString(), out int expenseId))
            {
                var expense = _userExpenses.FirstOrDefault(ex => ex.Id == expenseId);
                if (expense != null)
                {
                    // For now, just log the action
                    Console.WriteLine($"Delete expense request: {expense.ExpenseName} ({expense.RequiredAmount:C})");
                    
                    // Remove from list and refresh
                    _userExpenses.Remove(expense);
                    LoadData();
                }
            }
        }
    }
    
    private void OnPrevPageClick(object? sender, RoutedEventArgs e)
    {
        if (_currentPage > 1)
        {
            _currentPage--;
            UpdateExpenseGrid();
        }
    }
    
    private void OnNextPageClick(object? sender, RoutedEventArgs e)
    {
        var itemsPerPage = 10;
        var totalPages = (int)Math.Ceiling(_userExpenses.Count / (double)itemsPerPage);
        
        if (_currentPage < totalPages)
        {
            _currentPage++;
            UpdateExpenseGrid();
        }
    }
    
    private void OnViewBudgetClick(object? sender, RoutedEventArgs e)
    {
        // Navigate to budget page
        if (this.Parent is ContentControl contentControl)
        {
            contentControl.Content = new BudgetPage(
                new BudgetController(),
                new UserController(),
                _currentUser,
                new List<Budget>(),
                _userExpenses);
        }
    }

    private void OnDateFilterChanged(object? sender, DatePickerSelectedValueChangedEventArgs e)
    {
        // For now, just refresh the grid
        // In a real app, you would filter by date
        UpdateExpenseGrid();
    }

    private void InitializeComponent()
    {
        // This method should be implemented by Avalonia.Xaml.Interactivity
        // See: https://github.com/wieslawsoltes/AvaloniaBehaviors
        Avalonia.Markup.Xaml.AvaloniaXamlLoader.Load(this);
    }
}

