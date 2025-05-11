using Model.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace View.Views
{
    public class ExpenseView : IView
    {
        private ExpenseController _expenseController;
        private BudgetController _budgetController;
        private UserController _userController;
        private User _currentUser;
        private List<Expense> _expenses;
        private List<Budget> _budgets;

        public async Task Show()
        {
            await RefreshBudgets();
            await RefreshExpenses();
            while (true)
            {
                Console.Clear();
                Console.WriteLine("== Expense Dashboard ==");
                Console.WriteLine("1 - Show Expenses");
                Console.WriteLine("2 - Add New Expense");
                Console.WriteLine("3 - Get/Manage Expense");
                Console.WriteLine("4 - Update Expense");
                Console.WriteLine("5 - Delete Expense");
                Console.WriteLine("6 - Back");
                Console.Write("Enter choice: ");
                string input = Console.ReadLine();

                switch (input)
                {
                    case "1":
                        await ShowExpenses();
                        break;
                    case "2":
                        await AddNewExpense();
                        break;
                    case "3":
                        await GetAndManageExpense();
                        break;
                    case "4":
                        await UpdateExpense();
                        break;
                    case "5":
                        await DeleteExpense();
                        break;
                    case "6":
                        return;
                    default:
                        Console.WriteLine("Invalid option. Press Enter to try again.");
                        Console.ReadLine();
                        break;
                }
            }
        }

        public async Task Initialize(User obj)
        {
            _expenseController = new ExpenseController();
            _budgetController = new BudgetController();
            _userController = new UserController();
            _currentUser = obj;
            _expenses = new List<Expense>();
            _budgets = new List<Budget>();
            
            // Load initial data
        }

        private async Task ShowExpenses()
        {
            await RefreshExpenses();
            if (_expenses.Count == 0)
            {
                Console.WriteLine("No expenses found.");
            }
            else
            {
                Console.WriteLine("Expenses:");
                foreach (var e in _expenses)
                {
                    Console.WriteLine(e.ToString());
                }
            }

            Console.WriteLine("Press Enter to return to the menu...");
            Console.ReadLine();
        }

        private async Task AddNewExpense()
        {
            // Show available budgets first
            await RefreshBudgets();
            if (_budgets.Count == 0)
            {
                Console.WriteLine("No budgets available. Please create a budget first.");
                Console.WriteLine("Press Enter to return...");
                Console.ReadLine();
                return;
            }

            Console.WriteLine("Available Budgets:");
            foreach (var b in _budgets)
            {
                Console.WriteLine($"- {b.BudgetName} (ID: {b.Id})");
            }

            var newExpense = new Expense();
            newExpense.UserId = _currentUser.Id;

            Console.Write("Enter budget name for this expense: ");
            string budgetName = Console.ReadLine();

            var budget = _budgets.FirstOrDefault(b => b.BudgetName.Equals(budgetName, StringComparison.OrdinalIgnoreCase));
            if (budget == null)
            {
                Console.WriteLine("Budget not found.");
                Console.WriteLine("Press Enter to return...");
                Console.ReadLine();
                return;
            }
            newExpense.BudgetId = budget.Id;

            Console.Write("Enter expense name: ");
            newExpense.ExpenseName = Console.ReadLine();

            Console.Write("Enter required amount: ");
            if (!decimal.TryParse(Console.ReadLine(), out var requiredAmount))
            {
                Console.WriteLine("Invalid amount. Press Enter to return.");
                Console.ReadLine();
                return;
            }
            newExpense.RequiredAmount = requiredAmount;

            Console.Write("Enter spent amount (default 0): ");
            var spentInput = Console.ReadLine();
            newExpense.SpentAmount = string.IsNullOrWhiteSpace(spentInput) ? 0 : decimal.Parse(spentInput);

            newExpense.DateCycle = DateTime.Now;

            var (success, id, errors) = await _expenseController.TryAddExpense(newExpense);
            if (!success)
            {
                Console.WriteLine("Failed to add expense:");
                errors.ForEach(e => Console.WriteLine($"- {e}"));
            }
            else
            {
                Console.WriteLine($"Expense added successfully with ID: {id}");
                await RefreshExpenses();
            }

            Console.WriteLine("Press Enter to return to the menu...");
            Console.ReadLine();
        }
        private async Task GetAndManageExpense()
        {
            Console.Write("Enter expense name to manage: ");
            string expenseName = Console.ReadLine();

            var (success, expense, errors) = await _expenseController.TrySearchExpense(expenseName, _currentUser.Id);
            if (!success || expense == null)
            {
                Console.WriteLine("Expense not found:");
                errors.ForEach(e => Console.WriteLine($"- {e}"));
                Console.WriteLine("Press Enter to return...");
                Console.ReadLine();
                return;
            }

            Console.WriteLine($"Managing Expense: {expense.ExpenseName}");
            Console.WriteLine($"Current spent amount: {expense.SpentAmount}");
            Console.WriteLine($"Required amount: {expense.RequiredAmount}");
            Console.WriteLine("1 - Add money to spent amount");
            Console.WriteLine("2 - Remove money from spent amount");
            Console.WriteLine("3 - Set reminder");
            Console.WriteLine("4 - Back");
            Console.Write("Enter choice: ");

            string input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    Console.Write("Enter amount to add: ");
                    if (decimal.TryParse(Console.ReadLine(), out var addAmount))
                    {
                        expense.SpentAmount += addAmount;
                        await UpdateExpense(expense, skipPrompts: true);
                        Console.WriteLine("Amount added successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid amount.");
                    }
                    break;
                case "2":
                    Console.Write("Enter amount to remove: ");
                    if (decimal.TryParse(Console.ReadLine(), out var removeAmount))
                    {
                        expense.SpentAmount -= removeAmount;
                        await UpdateExpense(expense, skipPrompts: true);
                        Console.WriteLine("Amount removed successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Invalid amount.");
                    }
                    break;
                case "3":
                    Console.Write("Enter reminder date/time (MM/dd/yyyy HH:mm): ");
                    if (DateTime.TryParse(Console.ReadLine(), out var reminderTime))
                    {
                        var result = await _expenseController.TrySetExpenseReminder(expense,reminderTime);
                        if (result.Success)
                            Console.WriteLine("Reminder set successfully.");
                        else
                        {
                            Console.WriteLine("Failed to Set a Reminder");
                            errors.ForEach(e => Console.WriteLine($"- {e}"));
                            Console.WriteLine("Press Enter to return.");
                            Console.ReadLine(); 
                        }
                            
                    }
                    else
                    {
                        Console.WriteLine("Invalid date/time format.");
                    }
                    break;
                case "4":
                    return;
                default:
                    Console.WriteLine("Invalid option.");
                    break;
            }

            Console.WriteLine("Press Enter to return...");
            Console.ReadLine();
        }
        private async Task UpdateExpense(Expense expenseToUpdate = null, bool skipPrompts = false)
        {
            if (expenseToUpdate == null)
            {
                Console.Write("Enter the name of the expense to update: ");
                var name = Console.ReadLine();

                var found = await _expenseController.TrySearchExpense(name, _currentUser.Id);
                if (!found.Success || found.expense == null)
                {
                    Console.WriteLine("Expense not found.");
                    found.Errors.ForEach(e => Console.WriteLine($"- {e}"));
                    Console.WriteLine("Press Enter to return.");
                    Console.ReadLine();
                    return;
                }
                
                expenseToUpdate = found.expense;
            }

            // Only show prompts if we're not skipping them (like when called from GetAndManageExpense)
            if (!skipPrompts)
            {
                Console.Write("Enter new name (leave blank to keep current): ");
                string newName = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(newName))
                    expenseToUpdate.ExpenseName = newName;

                Console.Write("Enter new required amount (leave blank to keep current): ");
                var reqAmtInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(reqAmtInput) && decimal.TryParse(reqAmtInput, out var newReqAmount))
                    expenseToUpdate.RequiredAmount = newReqAmount;

                Console.Write("Enter new spent amount (leave blank to keep current): ");
                var spentAmtInput = Console.ReadLine();
                if (!string.IsNullOrWhiteSpace(spentAmtInput) && decimal.TryParse(spentAmtInput, out var newSpentAmount))
                    expenseToUpdate.SpentAmount = newSpentAmount;
            }

            var (success, updatedExpense, updateErrors) = await _expenseController.TryUpdateExpense(expenseToUpdate);
            if (!success)
            {
                Console.WriteLine("Failed to update:");
                updateErrors.ForEach(e => Console.WriteLine($"- {e}"));
            }
            else
            {
                Console.WriteLine("Expense updated successfully.");
                await RefreshExpenses();
            }

            if (!skipPrompts)
            {
                Console.WriteLine("Press Enter to return...");
                Console.ReadLine();
            }
        }

        private async Task DeleteExpense()
        {
            Console.Write("Enter the name of the expense to delete: ");
            var name = Console.ReadLine();

            var (success, expense, errors) = await _expenseController.TrySearchExpense(name, _currentUser.Id);
            if (!success || expense == null)
            {
                Console.WriteLine("Expense not found:");
                errors.ForEach(e => Console.WriteLine($"- {e}"));
                Console.WriteLine("Press Enter to return.");
                Console.ReadLine();
                return;
            }

            var (deleteSuccess, deleteErrors) = await _expenseController.TryDeleteExpense(expense);
            if (!deleteSuccess)
            {
                Console.WriteLine("Failed to delete:");
                deleteErrors.ForEach(e => Console.WriteLine($"- {e}"));
            }
            else
            {
                Console.WriteLine("Expense deleted successfully.");
                _expenses.Remove(expense); // Update local cache
            }

            Console.WriteLine("Press Enter to return...");
            Console.ReadLine();
        }

        private async Task RefreshExpenses()
        {
            var result = await _userController.TryGetUserExpenses(_currentUser);
            if (result.Success)
            {
                _expenses = result.Expenses;
            }
            else
            {
                Console.WriteLine("Couldn't Refresh Expenses");
                result.errors.ForEach(Console.WriteLine);
            }
        }

        private async Task RefreshBudgets()
        {
            var result = await _userController.TryGetUserBudgets(_currentUser);
            if (result.Success)
            {
                _budgets = result.Budgets;
            }
            else
            {
                Console.WriteLine("Couldn't Refresh Budgets");
                result.errors.ForEach(Console.WriteLine);
            }
        }
    }
}