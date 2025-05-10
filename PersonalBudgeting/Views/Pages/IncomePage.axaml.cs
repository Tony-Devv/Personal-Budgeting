using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
    private int _currentPage = 1;
    private const int _pageSize = 5;
    private int _totalPages = 1;
    private TextBlock? _currentPageText;
    private Button? _prevPageButton;
    private Button? _nextPageButton;

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
            
        if (_prevPageButton != null)
            _prevPageButton.Click += OnPrevPageClick;
            
        if (_nextPageButton != null)
            _nextPageButton.Click += OnNextPageClick;
            
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
            
        if (_prevPageButton != null)
            _prevPageButton.Click += OnPrevPageClick;
            
        if (_nextPageButton != null)
            _nextPageButton.Click += OnNextPageClick;
            
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
        _currentPageText = this.FindControl<TextBlock>("PageInfoText");
        _prevPageButton = this.FindControl<Button>("PrevPageButton");
        _nextPageButton = this.FindControl<Button>("NextPageButton");
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
    
    private void UpdateSummaryCards()
    {
        try
        {
            // Calculate total income
            var totalIncome = _userIncomes.Sum(i => i.Amount);
            
            // Calculate average income
            var avgIncome = _userIncomes.Any() ? _userIncomes.Average(i => i.Amount) : 0;
            
            // Count unique income sources
            var uniqueSources = _userIncomes.Select(i => i.IncomeSourceName).Distinct().Count();
            
            // Update UI
            if (_totalIncomeText != null)
                _totalIncomeText.Text = $"${totalIncome:N2}";
                
            if (_avgIncomeText != null)
                _avgIncomeText.Text = $"${avgIncome:N2}";
                
            if (_incomeSourcesText != null)
                _incomeSourcesText.Text = uniqueSources.ToString();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating summary cards: {ex.Message}");
        }
    }
    
    private void UpdateIncomeGrid()
    {
        try
        {
            if (_incomeListPanel == null || _currentPageText == null)
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
            
            // Calculate pagination
            _totalPages = (int)Math.Ceiling(filteredIncomes.Count / (double)_pageSize);
            _totalPages = Math.Max(1, _totalPages); // At least 1 page
            _currentPage = Math.Min(_currentPage, _totalPages);
            
            // Update current page text
            _currentPageText.Text = $"Page {_currentPage} of {_totalPages}";
            
            // Enable/disable pagination buttons
            if (_prevPageButton != null)
                _prevPageButton.IsEnabled = _currentPage > 1;
                
            if (_nextPageButton != null)
                _nextPageButton.IsEnabled = _currentPage < _totalPages;
            
            // Get current page items
            var pageItems = filteredIncomes
                .Skip((_currentPage - 1) * _pageSize)
                .Take(_pageSize)
                .ToList();
            
            // Create UI elements for each income
            foreach (var income in pageItems)
            {
                // Create new panel for each income entry
                var panel = new StackPanel
                {
                    Margin = new Avalonia.Thickness(0, 0, 0, 10)
                };
                
                // Create text blocks for income details
                var dateText = new TextBlock
                {
                    Text = income.IncomeDate.ToString("yyyy-MM-dd"),
                    FontWeight = Avalonia.Media.FontWeight.Bold
                };
                
                var descText = new TextBlock
                {
                    Text = income.IncomeSourceName,
                    Margin = new Avalonia.Thickness(0, 5, 0, 0)
                };
                
                var amountText = new TextBlock
                {
                    Text = $"${income.Amount:N2}",
                    Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Color.Parse("#4CAF50")),
                    FontWeight = Avalonia.Media.FontWeight.Bold,
                    Margin = new Avalonia.Thickness(0, 5, 0, 0)
                };
                
                // Create button panel for actions
                var buttonPanel = new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    Margin = new Avalonia.Thickness(0, 10, 0, 0),
                    Spacing = 10
                };
                
                // Create edit button
                var editButton = new Button
                {
                    Content = "Edit",
                    Tag = income.Id
                };
                editButton.Click += OnEditIncomeClick;
                
                // Create delete button
                var deleteButton = new Button
                {
                    Content = "Delete",
                    Tag = income.Id,
                    Foreground = Brushes.White,
                    Background = new SolidColorBrush(Color.Parse("#F44336"))
                };
                deleteButton.Click += OnDeleteIncomeClick;
                
                // Add buttons to panel
                buttonPanel.Children.Add(editButton);
                buttonPanel.Children.Add(deleteButton);
                
                // Add text blocks to panel
                panel.Children.Add(dateText);
                panel.Children.Add(descText);
                panel.Children.Add(amountText);
                panel.Children.Add(buttonPanel);
                
                // Add panel to the income list
                _incomeListPanel.Children.Add(panel);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error updating income grid: {ex.Message}");
        }
    }
    
    private void OnSearchTextChanged(object? sender, TextChangedEventArgs e)
    {
        // Reset to first page and update grid
        _currentPage = 1;
        UpdateIncomeGrid();
    }
    
    private void OnCategoryFilterChanged(object? sender, SelectionChangedEventArgs e)
    {
        // Reset to first page and update grid
        _currentPage = 1;
        UpdateIncomeGrid();
    }
    
    private void OnPrevPageClick(object? sender, RoutedEventArgs e)
    {
        if (_currentPage > 1)
        {
            _currentPage--;
            UpdateIncomeGrid();
        }
    }
    
    private void OnNextPageClick(object? sender, RoutedEventArgs e)
    {
        if (_currentPage < _totalPages)
        {
            _currentPage++;
            UpdateIncomeGrid();
        }
    }
    
    private void OnEditIncomeClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int incomeId)
        {
            // Find the income to edit
            var incomeToEdit = _userIncomes.FirstOrDefault(i => i.Id == incomeId);
            if (incomeToEdit != null)
            {
                // In a real app, show a dialog to edit the income
                Console.WriteLine($"Editing income with id {incomeId}");
            }
        }
    }
    
    private void OnDeleteIncomeClick(object? sender, RoutedEventArgs e)
    {
        if (sender is Button button && button.Tag is int incomeId)
        {
            // Find the income to delete
            var incomeToDelete = _userIncomes.FirstOrDefault(i => i.Id == incomeId);
            if (incomeToDelete != null)
            {
                // Remove from the list
                _userIncomes.Remove(incomeToDelete);
                
                // Log the deletion for debugging
                Console.WriteLine($"Deleted income with ID: {incomeId}");
                
                // Update UI
                UpdateSummaryCards();
                UpdateIncomeGrid();
            }
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
                
                if (sourceInput != null)
                    sourceInput.Text = string.Empty;
                
                if (amountInput != null)
                    amountInput.Text = string.Empty;
                
                if (datePicker != null)
                    datePicker.SelectedDate = DateTime.Today;
                
                if (errorText != null)
                    errorText.IsVisible = false;
                
                // Show popup
                popup.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error showing income popup: {ex.Message}");
        }
    }
    
    private void OnSaveIncomeClick(object? sender, RoutedEventArgs e)
    {
        try
        {
            // Get input values
            var sourceInput = this.FindControl<TextBox>("IncomeSourceInput");
            var amountInput = this.FindControl<TextBox>("IncomeAmountInput");
            var datePicker = this.FindControl<DatePicker>("IncomeDatePicker");
            var errorText = this.FindControl<TextBlock>("AddIncomeErrorText");
            var popup = this.FindControl<Border>("AddIncomePopup");
            
            if (sourceInput == null || amountInput == null || datePicker == null || errorText == null || popup == null)
                return;
            
            var source = sourceInput.Text?.Trim();
            var amountText = amountInput.Text?.Trim();
            var date = datePicker.SelectedDate ?? DateTimeOffset.Now;
            
            // Validate source
            if (string.IsNullOrWhiteSpace(source))
            {
                errorText.Text = "Income source is required";
                errorText.IsVisible = true;
                return;
            }
            
            // Validate amount
            if (!decimal.TryParse(amountText, out decimal amount) || amount <= 0)
            {
                errorText.Text = "Please enter a valid amount";
                errorText.IsVisible = true;
                return;
            }
            
            // Create income object 
            var income = new Income
            {
                UserId = _currentUser.Id,
                IncomeSourceName = source,
                Amount = amount,
                IncomeDate = date.DateTime
            };
            
            // Generate an ID for the new income (in a real app, this would come from the database)
            income.Id = _userIncomes.Any() ? _userIncomes.Max(i => i.Id) + 1 : 1;
            
            // Add to the local list
            _userIncomes.Add(income);
            
            // Update UI
            UpdateIncomeGrid();
            UpdateSummaryCards();
            
            // Hide popup
            popup.IsVisible = false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving income: {ex.Message}");
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
