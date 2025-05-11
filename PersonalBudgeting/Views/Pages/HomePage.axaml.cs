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
using PersonalBudgeting.ViewModels;

namespace PersonalBudgeting.Views.Pages;

public partial class HomePage : UserControl
{
    private readonly UserController _userController;
    private readonly User? _currentUser;
    private List<Income>? _userIncomes;
    private List<Expense>? _userExpenses;
    private readonly ContentControl? _contentFrame;
    
    // View Model
    public HomeViewModel ViewModel { get; }
    
    // UI Controls
    private TextBlock? _welcomeMessage;
    
    public HomePage()
    {
        InitializeComponent();
        
        // Create controllers for demo/design mode
        _userController = new UserController();
        _currentUser = new User { Id = 0, UserName = "Demo User" };
        
        // Initialize ViewModel
        ViewModel = new HomeViewModel();
        DataContext = ViewModel;
        
        // Connect to UI elements
        ConnectToUIElements();
        
        // Set welcome message
        if (_welcomeMessage != null)
        {
            _welcomeMessage.Text = "Welcome, Tony";
        }
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
        
        // Initialize ViewModel
        ViewModel = new HomeViewModel();
        DataContext = ViewModel;
        
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
    }
    
    private async Task LoadDashboardDataAsync()
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
                
                // Calculate total income from actual data
                var totalIncome = _userIncomes?.Sum(i => i.Amount) ?? 0;
                ViewModel.TotalIncome = totalIncome;
            }
            
            // Load expenses
            var expensesResult = await _userController.TryGetUserExpenses(_currentUser);
            if (expensesResult.Success)
            {
                _userExpenses = expensesResult.Expenses;
                
                // Calculate total expenses from actual data
                var totalExpenses = _userExpenses?.Sum(e => e.SpentAmount) ?? 0;
                ViewModel.TotalExpenses = totalExpenses;
            }
            
            // Set welcome message
            var welcomeMessage = this.Get<TextBlock>("WelcomeMessage");
            if (welcomeMessage != null && _currentUser != null)
            {
                welcomeMessage.Text = $"Welcome back, {_currentUser.UserName}";
            }
            
            // Update total amount values in the UI (in case the binding doesn't work)
            var totalBalanceText = this.Get<TextBlock>("TotalBalanceText");
            var totalIncomeText = this.Get<TextBlock>("TotalIncomeText");
            var totalExpensesText = this.Get<TextBlock>("TotalExpensesText");
            
            if (totalBalanceText != null)
            {
                totalBalanceText.Text = ViewModel.TotalBalance.ToString("C");
            }
            
            if (totalIncomeText != null)
            {
                totalIncomeText.Text = ViewModel.TotalIncome.ToString("C");
            }
            
            if (totalExpensesText != null)
            {
                totalExpensesText.Text = ViewModel.TotalExpenses.ToString("C");
            }
        }
        catch (Exception ex)
        {
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
