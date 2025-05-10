using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Controller;
using Model.Entities;

namespace PersonalBudgeting.Views.Pages;

public partial class HomePage : UserControl
{
    private readonly UserController _userController;
    private readonly User? _currentUser;
    private List<Income>? _userIncomes;
    private List<Expense>? _userExpenses;
    private List<Budget>? _userBudgets;
    private readonly ContentControl? _contentFrame;
    
    // UI Controls
    private TextBlock? _welcomeMessage;
    private TextBlock? _totalIncomeText;
    private TextBlock? _totalExpensesText;
    private TextBlock? _totalBudgetText;
    private TextBlock? _budgetInfoText;
    
    public HomePage()
    {
        InitializeComponent();
        
        // Create controllers for demo/design mode
        _userController = new UserController();
        _currentUser = new User { Id = 0, UserName = "Demo User" };
        
        // Connect to UI elements
        ConnectToUIElements();
        
        // Load dummy data for design mode
        LoadDummyData();
    }
    
    public HomePage(
        UserController userController,
        User currentUser,
        ContentControl contentFrame)
    {
        InitializeComponent();
        
        _userController = userController ?? throw new ArgumentNullException(nameof(userController));
        _currentUser = currentUser ?? throw new ArgumentNullException(nameof(currentUser));
        _contentFrame = contentFrame ?? throw new ArgumentNullException(nameof(contentFrame));
        
        // Connect to UI elements
        ConnectToUIElements();
        
        // Set welcome message
        if (_welcomeMessage != null && _currentUser != null)
        {
            _welcomeMessage.Text = $"Welcome, {_currentUser.UserName}";
        }
        
        // Load real data
        LoadDashboardDataAsync();
    }
    
    private void ConnectToUIElements()
    {
        _welcomeMessage = this.FindControl<TextBlock>("WelcomeMessage");
        _totalIncomeText = this.FindControl<TextBlock>("TotalIncomeText");
        _totalExpensesText = this.FindControl<TextBlock>("TotalExpensesText");
        _totalBudgetText = this.FindControl<TextBlock>("TotalBudgetText");
        _budgetInfoText = this.FindControl<TextBlock>("BudgetInfoText");
    }
    
    private void LoadDummyData()
    {
        // Set welcome message
        if (_welcomeMessage != null)
        {
            _welcomeMessage.Text = "Welcome, Demo User";
        }
        
        // Set dummy values for stats
        if (_totalIncomeText != null)
        {
            _totalIncomeText.Text = "$5,200.00";
        }
        
        if (_totalExpensesText != null)
        {
            _totalExpensesText.Text = "$3,450.00";
        }
        
        if (_totalBudgetText != null)
        {
            _totalBudgetText.Text = "$1,750.00";
        }
        
        if (_budgetInfoText != null)
        {
            _budgetInfoText.Text = "Available Balance";
        }
    }
    
    private async void LoadDashboardDataAsync()
    {
        try
        {
            if (_currentUser == null || _userController == null)
            {
                return;
            }
            
            // Load incomes
            var incomesResult = await _userController.TryGetUserIncomes(_currentUser);
            if (incomesResult.Success)
            {
                _userIncomes = incomesResult.Incomes;
                
                // Calculate total income
                var totalIncome = _userIncomes?.Sum(i => i.Amount) ?? 0;
                if (_totalIncomeText != null)
                {
                    _totalIncomeText.Text = totalIncome.ToString("C");
                }
            }
            
            // Load expenses
            var expensesResult = await _userController.TryGetUserExpenses(_currentUser);
            if (expensesResult.Success)
            {
                _userExpenses = expensesResult.Expenses;
                
                // Calculate total expenses
                var totalExpenses = _userExpenses?.Sum(e => e.SpentAmount) ?? 0;
                if (_totalExpensesText != null)
                {
                    _totalExpensesText.Text = totalExpenses.ToString("C");
                }
            }
            
            // Load budgets
            var budgetsResult = await _userController.TryGetUserBudgets(_currentUser);
            if (budgetsResult.Success)
            {
                _userBudgets = budgetsResult.Budgets;
                
                // Calculate total budget and remaining budget
                var totalBudget = _userBudgets?.Sum(b => b.TotalAmountRequired) ?? 0;
                var totalExpenses = _userExpenses?.Sum(e => e.SpentAmount) ?? 0;
                var remainingBudget = totalBudget - totalExpenses;
                
                if (_totalBudgetText != null)
                {
                    _totalBudgetText.Text = remainingBudget.ToString("C");
                }
                
                if (_budgetInfoText != null)
                {
                    if (remainingBudget < 0)
                    {
                        _budgetInfoText.Text = "Over Budget";
                    }
                    else if (remainingBudget < (totalBudget * 0.2m))
                    {
                        _budgetInfoText.Text = "Low Budget";
                    }
                    else
                    {
                        _budgetInfoText.Text = "Available Balance";
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading dashboard data: {ex.Message}");
        }
    }
    
    private void OnIncomeNavigationClick(object? sender, RoutedEventArgs e)
    {
        if (_contentFrame != null && _userController != null && _currentUser != null)
        {
            var incomeController = new IncomeController();
            var userIncomes = _userIncomes ?? new List<Income>();
            _contentFrame.Content = new IncomePage(incomeController, _userController, _currentUser, userIncomes);
        }
    }
    
    private void OnExpensesNavigationClick(object? sender, RoutedEventArgs e)
    {
        if (_contentFrame != null && _userController != null && _currentUser != null)
        {
            var expenseController = new ExpenseController();
            _contentFrame.Content = new ExpensesPage(expenseController, _userController, _currentUser);
        }
    }
    
    private void OnBudgetNavigationClick(object? sender, RoutedEventArgs e)
    {
        if (_contentFrame != null && _userController != null && _currentUser != null)
        {
            var budgetController = new BudgetController();
            _contentFrame.Content = new BudgetPage(budgetController, _userController, _currentUser);
        }
    }
    
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}
