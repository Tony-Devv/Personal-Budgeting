using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Controller;
using Model.Entities;
using PersonalBudgeting.Views.Pages;

namespace PersonalBudgeting.Views.Pages;

public partial class BudgetPage : UserControl
{
    private readonly BudgetController? _budgetController;
    private readonly UserController? _userController;
    private readonly User? _currentUser;
    private readonly List<Budget>? _userBudgets;
    private readonly List<Expense>? _userExpenses;
    private string _searchText = string.Empty;
    private string _selectedStatus = "All Status";

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
        User currentUser,
        List<Budget> userBudgets,
        List<Expense> userExpenses)
    {
        InitializeComponent();
        
        _budgetController = budgetController;
        _userController = userController;
        _currentUser = currentUser;
        _userBudgets = userBudgets ?? new List<Budget>();
        _userExpenses = userExpenses ?? new List<Expense>();

        LoadData();
    }

    private void LoadData()
    {
        UpdateSummaryCards();
        UpdateBudgetOverview();
        UpdateBudgetGrid();
    }

    private void UpdateSummaryCards()
    {
        // Calculate total budget
        var totalBudget = _userBudgets != null ? (double)_userBudgets.Sum(b => b.TotalAmountRequired) : 0;
        var totalBudgetText = this.FindControl<TextBlock>("TotalBudgetText");
        if (totalBudgetText != null)
        {
            totalBudgetText.Text = totalBudget.ToString("C");
        }

        // Calculate budget used (total expenses)
        var totalExpenses = _userExpenses != null ? (double)_userExpenses.Sum(e => e.RequiredAmount) : 0;
        var budgetUsedText = this.FindControl<TextBlock>("BudgetUsedText");
        if (budgetUsedText != null)
        {
            budgetUsedText.Text = totalExpenses.ToString("C");
        }

        // Calculate percentage of budget used
        var budgetUsedPercentage = totalBudget > 0 ? (totalExpenses / totalBudget) * 100 : 0;
        var budgetUsedPercentageText = this.FindControl<TextBlock>("BudgetUsedPercentageText");
        if (budgetUsedPercentageText != null)
        {
            budgetUsedPercentageText.Text = $"{budgetUsedPercentage:F1}%";
        }

        // Calculate remaining budget
        var remainingBudget = totalBudget - totalExpenses;
        var remainingBudgetText = this.FindControl<TextBlock>("RemainingBudgetText");
        if (remainingBudgetText != null)
        {
            remainingBudgetText.Text = remainingBudget.ToString("C");
        }

        // Update the status text
        var daysRemainingText = this.FindControl<TextBlock>("DaysRemainingText");
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

    private void UpdateBudgetOverview()
    {
        // Calculate total budget
        var totalBudget = _userBudgets != null ? (double)_userBudgets.Sum(b => b.TotalAmountRequired) : 0;
        
        // Calculate total expenses
        var totalExpenses = _userExpenses != null ? (double)_userExpenses.Sum(e => e.RequiredAmount) : 0;
        
        // Calculate percentage of budget used
        var budgetUsedPercentage = totalBudget > 0 ? (totalExpenses / totalBudget) * 100 : 0;
        
        // Calculate remaining budget
        var remainingBudget = totalBudget - totalExpenses;
        
        // Update progress bar
        var overallProgressBar = this.FindControl<ProgressBar>("OverallProgressBar");
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
        var overallProgressText = this.FindControl<TextBlock>("OverallProgressText");
        if (overallProgressText != null)
        {
            overallProgressText.Text = $"{budgetUsedPercentage:F1}%";
        }
        
        var chartBudgetText = this.FindControl<TextBlock>("ChartBudgetText");
        if (chartBudgetText != null)
        {
            chartBudgetText.Text = totalBudget.ToString("C");
        }
        
        var chartSpentText = this.FindControl<TextBlock>("ChartSpentText");
        if (chartSpentText != null)
        {
            chartSpentText.Text = totalExpenses.ToString("C");
        }
        
        var chartRemainingText = this.FindControl<TextBlock>("ChartRemainingText");
        if (chartRemainingText != null)
        {
            chartRemainingText.Text = remainingBudget.ToString("C");
        }
    }

    private void UpdateBudgetGrid()
    {
        // Get the StackPanel that will hold the budget items
        var budgetItemsPanel = this.FindControl<StackPanel>("BudgetItemsPanel");
        if (budgetItemsPanel == null) return;
        
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
            
            // Add the row to the panel
            budgetItemsPanel.Children.Add(rowGrid);
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
        // Todo: Add dialog for creating a budget
        Console.WriteLine("Create budget button clicked");
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
}
