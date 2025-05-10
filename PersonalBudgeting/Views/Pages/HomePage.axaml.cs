using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Controller;
using Model.Entities;
using Avalonia;
using Avalonia.Markup.Xaml;

namespace PersonalBudgeting.Views.Pages;

public partial class HomePage : UserControl, INotifyPropertyChanged
{
    private readonly BudgetController _budgetController;
    private readonly ExpenseController _expenseController;
    private readonly IncomeController _incomeController;
    private readonly UserController _userController;
    private readonly User? _currentUser;
    private readonly List<Income>? _userIncomes;
    private readonly List<Expense>? _userExpenses;
    private readonly List<Budget>? _userBudgets;
    
    // Bindable properties
    private string _totalIncome = "$0.00";
    public string TotalIncome
    {
        get => _totalIncome;
        set
        {
            if (_totalIncome != value)
            {
                _totalIncome = value;
                OnPropertyChanged();
            }
        }
    }
    
    private string _totalExpenses = "$0.00";
    public string TotalExpenses
    {
        get => _totalExpenses;
        set
        {
            if (_totalExpenses != value)
            {
                _totalExpenses = value;
                OnPropertyChanged();
            }
        }
    }
    
    private string _incomeChange = "+0%";
    public string IncomeChange
    {
        get => _incomeChange;
        set
        {
            if (_incomeChange != value)
            {
                _incomeChange = value;
                OnPropertyChanged();
            }
        }
    }
    
    private string _expenseChange = "+0%";
    public string ExpenseChange
    {
        get => _expenseChange;
        set
        {
            if (_expenseChange != value)
            {
                _expenseChange = value;
                OnPropertyChanged();
            }
        }
    }
    
    private string _budgetStatus = "On Track";
    public string BudgetStatus
    {
        get => _budgetStatus;
        set
        {
            if (_budgetStatus != value)
            {
                _budgetStatus = value;
                OnPropertyChanged();
            }
        }
    }
    
    private string _budgetRemaining = "$0.00";
    public string BudgetRemaining
    {
        get => _budgetRemaining;
        set
        {
            if (_budgetRemaining != value)
            {
                _budgetRemaining = value;
                OnPropertyChanged();
            }
        }
    }
    
    private ObservableCollection<TransactionViewModel> _transactions = new();
    public ObservableCollection<TransactionViewModel> Transactions
    {
        get => _transactions;
        set
        {
            if (_transactions != value)
            {
                _transactions = value;
                OnPropertyChanged();
            }
        }
    }
    
    // For filtered transactions
    private string _currentTransactionFilter = "All";

    private TextBlock? _welcomeMessage;
    private TextBlock? _totalIncomeText;
    private TextBlock? _totalExpensesText;
    private TextBlock? _activeBudgetsText;
    private ItemsControl? _recentTransactionsList;
    private Panel? _chartContainer;
    private ComboBox? _timeRangeComboBox;

    public HomePage()
    {
        InitializeComponent();
        
        // Initialize controllers
        _userController = new UserController();
        _expenseController = new ExpenseController();
        _incomeController = new IncomeController();
        _budgetController = new BudgetController();
        _userIncomes = new List<Income>();
        _userExpenses = new List<Expense>();
        _userBudgets = new List<Budget>();
        
        // Get control references
        _welcomeMessage = this.FindControl<TextBlock>("WelcomeMessage");
        _totalIncomeText = this.FindControl<TextBlock>("TotalIncomeText");
        _totalExpensesText = this.FindControl<TextBlock>("TotalExpensesText");
        _activeBudgetsText = this.FindControl<TextBlock>("ActiveBudgetsText");
        _recentTransactionsList = this.FindControl<ItemsControl>("RecentTransactionsList");
        _chartContainer = this.FindControl<Panel>("ChartContainer");
        _timeRangeComboBox = this.FindControl<ComboBox>("TimeRangeComboBox");
        
        // Add event handlers
        if (_timeRangeComboBox != null)
        {
            _timeRangeComboBox.SelectionChanged += OnTimeRangeChanged;
        }
        
        // Load initial data
        LoadData();
    }

    public HomePage(
        BudgetController budgetController,
        ExpenseController expenseController,
        IncomeController incomeController,
        UserController userController,
        User currentUser,
        List<Income> userIncomes,
        List<Expense> userExpenses,
        List<Budget> userBudgets)
    {
        InitializeComponent();
        
        _budgetController = budgetController;
        _expenseController = expenseController;
        _incomeController = incomeController;
        _userController = userController;
        _currentUser = currentUser;
        _userIncomes = userIncomes;
        _userExpenses = userExpenses;
        _userBudgets = userBudgets;
        
        LoadData();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public void LoadData()
    {
        // Set welcome message if user exists
        if (_currentUser != null && _welcomeMessage != null)
        {
            _welcomeMessage.Text = $"Welcome, {_currentUser.UserName}";
        }
        
        // Load financial data from the user
        LoadFinancialSummary();
        LoadRecentTransactions();
        InitializeChart();
    }

    private async void LoadFinancialSummary()
    {
        try
        {
            decimal totalIncome = 0;
            decimal totalExpenses = 0;
            int activeBudgets = 0;
            decimal budgetRemaining = 0;
            
            // If we don't have the user data yet, try to fetch it
            List<Income> incomes = new List<Income>();
            List<Expense> expenses = new List<Expense>();
            List<Budget> budgets = new List<Budget>();
            
            if (_currentUser != null)
            {
                if (_userIncomes == null || _userIncomes.Count == 0)
                {
                    var (success, userIncomes, errors) = await _userController.TryGetUserIncomes(_currentUser);
                    if (success)
                    {
                        incomes = userIncomes;
                    }
                }
                else
                {
                    incomes = _userIncomes;
                }
                
                if (_userExpenses == null || _userExpenses.Count == 0)
                {
                    var (success, userExpenses, errors) = await _userController.TryGetUserExpenses(_currentUser);
                    if (success)
                    {
                        expenses = userExpenses;
                    }
                }
                else
                {
                    expenses = _userExpenses;
                }
                
                if (_userBudgets == null || _userBudgets.Count == 0)
                {
                    var (success, userBudgets, errors) = await _userController.TryGetUserBudgets(_currentUser);
                    if (success)
                    {
                        budgets = userBudgets;
                    }
                }
                else
                {
                    budgets = _userBudgets;
                }
            }
            
            // Calculate income total
            totalIncome = incomes.Sum(i => i.Amount);
            
            // Calculate expense total
            totalExpenses = expenses.Sum(e => e.RequiredAmount);
            
            // Calculate budget metrics
            activeBudgets = budgets.Count;
            budgetRemaining = budgets.Sum(b => b.TotalAmountRequired) - expenses.Sum(e => e.RequiredAmount);
            
            // Calculate percentage changes (from previous month)
            DateTime lastMonth = DateTime.Now.AddMonths(-1);
            decimal lastMonthIncome = incomes
                .Where(i => i.IncomeDate.Month == lastMonth.Month && i.IncomeDate.Year == lastMonth.Year)
                .Sum(i => i.Amount);
            
            decimal lastMonthExpenses = expenses
                .Where(e => e.DateCycle.Month == lastMonth.Month && e.DateCycle.Year == lastMonth.Year)
                .Sum(e => e.RequiredAmount);
            
            decimal currentMonthIncome = incomes
                .Where(i => i.IncomeDate.Month == DateTime.Now.Month && i.IncomeDate.Year == DateTime.Now.Year)
                .Sum(i => i.Amount);
            
            decimal currentMonthExpenses = expenses
                .Where(e => e.DateCycle.Month == DateTime.Now.Month && e.DateCycle.Year == DateTime.Now.Year)
                .Sum(e => e.RequiredAmount);
            
            // Calculate percentage changes
            string incomeChangeText = "+0%";
            string expenseChangeText = "+0%";
            
            if (lastMonthIncome > 0)
            {
                decimal percentChange = ((currentMonthIncome - lastMonthIncome) / lastMonthIncome) * 100;
                string sign = percentChange >= 0 ? "+" : "";
                incomeChangeText = $"{sign}{percentChange:0}%";
            }
            
            if (lastMonthExpenses > 0)
            {
                decimal percentChange = ((currentMonthExpenses - lastMonthExpenses) / lastMonthExpenses) * 100;
                string sign = percentChange >= 0 ? "+" : "";
                expenseChangeText = $"{sign}{percentChange:0}%";
            }
            
            // Set budget status text
            string budgetStatusText = "On Track";
            if (totalExpenses > totalIncome)
            {
                budgetStatusText = "Over Budget";
            }
            
            // Update UI
            TotalIncome = $"${totalIncome:N2}";
            TotalExpenses = $"${totalExpenses:N2}";
            IncomeChange = incomeChangeText;
            ExpenseChange = expenseChangeText;
            BudgetStatus = budgetStatusText;
            BudgetRemaining = $"${budgetRemaining:N2}";
            
            // Update TextBlocks directly
            if (_totalIncomeText != null)
                _totalIncomeText.Text = TotalIncome;
            
            if (_totalExpensesText != null)
                _totalExpensesText.Text = TotalExpenses;
            
            if (_activeBudgetsText != null)
                _activeBudgetsText.Text = activeBudgets.ToString();
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Error loading financial summary: {ex.Message}");
        }
    }

    private async void LoadRecentTransactions()
    {
        try
        {
            List<TransactionViewModel> transactions = new List<TransactionViewModel>();
            
            List<Income> incomes = new List<Income>();
            List<Expense> expenses = new List<Expense>();
            
            // Get data if needed
            if (_currentUser != null)
            {
                if (_userIncomes == null || _userIncomes.Count == 0)
                {
                    var (success, userIncomes, errors) = await _userController.TryGetUserIncomes(_currentUser);
                    if (success)
                    {
                        incomes = userIncomes;
                    }
                }
                else
                {
                    incomes = _userIncomes;
                }
                
                if (_userExpenses == null || _userExpenses.Count == 0)
                {
                    var (success, userExpenses, errors) = await _userController.TryGetUserExpenses(_currentUser);
                    if (success)
                    {
                        expenses = userExpenses;
                    }
                }
                else
                {
                    expenses = _userExpenses;
                }
            }
            
            // Convert incomes to transaction view models
            foreach (var income in incomes)
            {
                transactions.Add(new TransactionViewModel
                {
                    Date = income.IncomeDate.ToString("MM/dd/yyyy"),
                    Description = income.IncomeSourceName,
                    Category = "Income", // Default category since Income doesn't have Category
                    Amount = $"+${income.Amount:N2}",
                    Type = "Income",
                    TypeColor = "#28a745" // Green color for income
                });
            }
            
            // Convert expenses to transaction view models
            foreach (var expense in expenses)
            {
                transactions.Add(new TransactionViewModel
                {
                    Date = expense.DateCycle.ToString("MM/dd/yyyy"),
                    Description = expense.ExpenseName,
                    Category = "Expense", // Default category since Expense doesn't have Category
                    Amount = $"-${expense.RequiredAmount:N2}",
                    Type = "Expense",
                    TypeColor = "#dc3545" // Red color for expense
                });
            }
            
            // Sort by date (newest first) and take the most recent 10
            var recentTransactions = transactions
                .OrderByDescending(t => DateTime.Parse(t.Date))
                .Take(10)
                .ToList();
            
            // Update the observable collection
            Transactions = new ObservableCollection<TransactionViewModel>(recentTransactions);
            
            // Update the ItemsControl directly
            if (_recentTransactionsList != null)
            {
                _recentTransactionsList.ItemsSource = Transactions;
            }
        }
        catch (Exception ex)
        {
            // Log the exception
            Console.WriteLine($"Error loading recent transactions: {ex.Message}");
        }
    }

    private void InitializeChart()
    {
        // This would typically involve creating a chart control
        // For now, we'll just add a placeholder
        if (_chartContainer != null)
        {
            // Clear existing children
            _chartContainer.Children.Clear();
            
            // In a real implementation, you would create a chart here
            // using a charting library
            var textBlock = new TextBlock
            {
                Text = "Chart will be displayed here",
                VerticalAlignment = Avalonia.Layout.VerticalAlignment.Center,
                HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center
            };
            
            _chartContainer.Children.Add(textBlock);
        }
    }

    private void OnTimeRangeChanged(object? sender, SelectionChangedEventArgs e)
    {
        // Update chart based on selected time range
        if (_timeRangeComboBox != null && _timeRangeComboBox.SelectedIndex >= 0)
        {
            // In a real implementation, you would update the chart data
            // based on the selected time range
            Console.WriteLine($"Time range changed to: {_timeRangeComboBox.SelectedIndex}");
        }
    }

    private void OnViewAllTransactionsClick(object? sender, RoutedEventArgs e)
    {
        // Navigate to transactions page (not implemented yet)
    }

    private void OnAddIncomeClick(object? sender, RoutedEventArgs e)
    {
        // Show add income dialog (not implemented yet)
    }

    private void OnAddExpenseClick(object? sender, RoutedEventArgs e)
    {
        // Show add expense dialog (not implemented yet)
    }

    private void OnViewBudgetClick(object? sender, RoutedEventArgs e)
    {
        // Navigate to budget page using MainWindow navigation
        var mainWindow = TopLevel.GetTopLevel(this) as MainWindow;
        if (mainWindow != null)
        {
            // Use reflection to access the NavigateToPage method since it's protected
            var method = mainWindow.GetType().GetMethod("NavigateToPage", 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Public);
            
            if (method != null)
            {
                method.Invoke(mainWindow, new object[] { "budget" });
            }
        }
    }
    
    private void OnViewIncomeClick(object? sender, RoutedEventArgs e)
    {
        // Navigate to income page using MainWindow navigation
        var mainWindow = TopLevel.GetTopLevel(this) as MainWindow;
        if (mainWindow != null)
        {
            // Use reflection to access the NavigateToPage method since it's protected
            var method = mainWindow.GetType().GetMethod("NavigateToPage", 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Public);
            
            if (method != null)
            {
                method.Invoke(mainWindow, new object[] { "income" });
            }
        }
    }
    
    private void OnViewExpensesClick(object? sender, RoutedEventArgs e)
    {
        // Navigate to expenses page using MainWindow navigation
        var mainWindow = TopLevel.GetTopLevel(this) as MainWindow;
        if (mainWindow != null)
        {
            // Use reflection to access the NavigateToPage method since it's protected
            var method = mainWindow.GetType().GetMethod("NavigateToPage", 
                System.Reflection.BindingFlags.Instance | 
                System.Reflection.BindingFlags.NonPublic | 
                System.Reflection.BindingFlags.Public);
            
            if (method != null)
            {
                method.Invoke(mainWindow, new object[] { "expenses" });
            }
        }
    }
    
    private void OnTransactionFilterChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ComboBox comboBox && comboBox.SelectedItem is ComboBoxItem selectedItem)
        {
            _currentTransactionFilter = selectedItem.Content?.ToString() ?? "All";
            
            // Filter transactions logic would go here
        }
    }
    
    private void OnChartPeriodChanged(object? sender, SelectionChangedEventArgs e)
    {
        // Update chart data based on selected period (not implemented yet)
    }
    
    // INotifyPropertyChanged implementation
    public new event PropertyChangedEventHandler? PropertyChanged;
    
    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

// Classes for transaction display
public class TransactionViewModel
{
    public string Date { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Amount { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string TypeColor { get; set; } = string.Empty;
}

public class TransactionItem
{
    public DateTime Date { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Type { get; set; } = string.Empty;
    public string TypeColor { get; set; } = string.Empty;
}
