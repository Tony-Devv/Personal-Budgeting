using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Linq;
using System.Collections.ObjectModel;

namespace PersonalBudgeting.ViewModels
{
    public class HomeViewModel : INotifyPropertyChanged
    {
        private string _welcomeMessage = "Welcome back";
        private string _currentMonth = "May 2025";
        private decimal _totalBalance = 0.0m;
        private decimal _totalIncome = 0.0m;
        private decimal _totalExpenses = 0.0m;
        private decimal _balancePercentChange;
        private decimal _incomePercentChange;
        private decimal _expensesPercentChange;
        
        public string WelcomeMessage 
        { 
            get => _welcomeMessage; 
            set 
            { 
                _welcomeMessage = value; 
                OnPropertyChanged(); 
            } 
        }
        
        public string CurrentMonth 
        { 
            get => _currentMonth; 
            set 
            { 
                _currentMonth = value; 
                OnPropertyChanged(); 
            } 
        }
        
        public decimal TotalBalance 
        { 
            get => _totalBalance; 
            set 
            { 
                _totalBalance = value; 
                OnPropertyChanged(); 
            } 
        }
        
        public decimal TotalIncome 
        { 
            get => _totalIncome; 
            set 
            { 
                _totalIncome = value; 
                OnPropertyChanged();
                // Calculate balance whenever income or expenses change
                TotalBalance = _totalIncome - _totalExpenses;
            } 
        }
        
        public decimal TotalExpenses 
        { 
            get => _totalExpenses; 
            set 
            { 
                _totalExpenses = value; 
                OnPropertyChanged();
                // Calculate balance whenever income or expenses change
                TotalBalance = _totalIncome - _totalExpenses;
            } 
        }
        
        public decimal BalancePercentChange
        {
            get => _balancePercentChange;
            set
            {
                if (_balancePercentChange != value)
                {
                    _balancePercentChange = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public decimal IncomePercentChange
        {
            get => _incomePercentChange;
            set
            {
                if (_incomePercentChange != value)
                {
                    _incomePercentChange = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public decimal ExpensesPercentChange
        {
            get => _expensesPercentChange;
            set
            {
                if (_expensesPercentChange != value)
                {
                    _expensesPercentChange = value;
                    OnPropertyChanged();
                }
            }
        }
        
        public event PropertyChangedEventHandler? PropertyChanged;
        
        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
    
    public class BudgetCategory
    {
        public string Name { get; set; } = "";
        public decimal Spent { get; set; }
        public decimal Budget { get; set; }
        public int Percentage { get; set; }
        public bool IsWarning { get; set; }
        public bool IsSuccess { get; set; }
        public bool IsDanger { get; set; }
        
        public string FormattedAmount => $"${Spent:N0} / ${Budget:N0}";
    }
} 
