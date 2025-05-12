using Model.Entities;

namespace View.Views;

public class BudgetView : IView
{
    private BudgetController _budgetController;
    private UserController _userController;
    private User _currentUser;
    private List<Budget> _budgets;

    public async Task Show()
    {
        await RefreshBudgets();
        while (true)
        {
            Console.Clear();
            Console.WriteLine("== Budget Dashboard ==");
            Console.WriteLine("1 - Show Budgets");
            Console.WriteLine("2 - Add New Budget");
            Console.WriteLine("3 - Edit Certain Budget");
            Console.WriteLine("4 - Delete Budget");
            Console.WriteLine("5 - Back");
            Console.Write("Enter choice: ");
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    await ShowBudgets();
                    break;
                case "2":
                    await AddNewBudget();
                    break;
                case "3":
                    await EditBudget();
                    break;
                case "4":
                    await DeleteBudget();
                    break;
                case "5":
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
        _budgetController = new BudgetController();
        _userController = new UserController();
        _currentUser = obj;
        _budgets = new List<Budget>();
    }

    private async Task ShowBudgets()
    {
        var (success, budgets, errors) = await _userController.TryGetUserBudgets(_currentUser);
        if (!success)
        {
            Console.WriteLine("Error fetching budgets:");
            errors.ForEach(e => Console.WriteLine($"- {e}"));
        }
        else
        {
            _budgets = budgets;
            if (_budgets.Count == 0)
            {
                Console.WriteLine("No budgets found.");
            }
            else
            {
                Console.WriteLine("Budgets:");
                foreach (var b in _budgets)
                {
                    var totalAmountSpent =
                        (await _userController.TryGetTotalAmountSpentOnBudget(_currentUser.Id, b.Id)).Total;
                    Console.WriteLine("---------------------------------------------");
                    Console.WriteLine(b.ToString());
                    Console.WriteLine($"Total Amount Spent on it: {totalAmountSpent}");
                    Console.WriteLine("---------------------------------------------");
                }
            }
        }

        Console.WriteLine("Press Enter to return to the menu...");
        Console.ReadLine();
    }

    private async Task AddNewBudget()
    {
        var newBudget = new Budget();
        newBudget.UserId = _currentUser.Id;

        Console.Write("Enter budget name: ");
        newBudget.BudgetName = Console.ReadLine();

        Console.Write("Enter Allowed Money Required to Spend: ");
        if (!decimal.TryParse(Console.ReadLine(), out var amount))
        {
            Console.WriteLine("Invalid amount. Press Enter to return.");
            Console.ReadLine();
            return;
        }
        newBudget.TotalAmountRequired = amount;

        
        var (success, id, errors) = await _budgetController.TryAddBudget(newBudget,true);
        if (!success)
        {
            Console.WriteLine("Failed to add budget:");
            errors.ForEach(e => Console.WriteLine($"- {e}"));
        }
        else
        {
            Console.WriteLine($"Budget added successfully with ID: {id}");
            await RefreshBudgets();
        }

        Console.WriteLine("Press Enter to return to the menu...");
        Console.ReadLine();
    }

    private async Task EditBudget()
    {
        
        Console.Write("Enter the name of the budget to edit: ");
        var name = Console.ReadLine();

        var found = await _budgetController.TryGetBudgetByName(_currentUser.Id, name,true);
        if (!found.Success)
        {
            Console.WriteLine("Budget not found.");
            found.errors.ForEach(e => Console.WriteLine($"- {e}"));
            Console.WriteLine("Press Enter to return.");
            Console.ReadLine();
            return;
        }
        
        var budgetToEdit = _budgets.FirstOrDefault(b => b.BudgetName == name);
        if (budgetToEdit == null)
        {
            Console.WriteLine("Budget name does not exist.");
            Console.ReadLine();
            return;
        }

        Console.Write("Enter new name (leave blank to keep current): ");
        string newName = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newName))
            budgetToEdit.BudgetName = newName;

        Console.Write("Enter new TotalRequiredAmount (leave blank to keep current): ");
        var amtInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(amtInput) && decimal.TryParse(amtInput, out var newAmount))
            budgetToEdit.TotalAmountRequired = newAmount;

        var (success, updatedBudget, updateErrors) = await _budgetController.TryUpdateBudget(budgetToEdit,true);
        if (!success)
        {
            Console.WriteLine("Failed to update:");
            updateErrors.ForEach(e => Console.WriteLine($"- {e}"));
        }
        else
        {
            Console.WriteLine("Budget updated successfully.");
        }

        Console.WriteLine("Press Enter to return...");
        Console.ReadLine();
    }

    private async Task DeleteBudget()
    {
        Console.Write("Enter the name of the budget to delete: ");
        var name = Console.ReadLine();

        var budgetToDelete = _budgets.FirstOrDefault(b => b.BudgetName == name);
        if (budgetToDelete == null)
        {
            Console.WriteLine("Budget not found.");
            Console.ReadLine();
            return;
        }

        var (success, errors) = await _budgetController.TryDeleteBudget(budgetToDelete,true);
        if (!success)
        {
            Console.WriteLine("Failed to delete:");
            errors.ForEach(e => Console.WriteLine($"- {e}"));
        }
        else
        {
            Console.WriteLine("Budget deleted successfully.");
            _budgets.Remove(budgetToDelete); // Update local cache
        }

        Console.WriteLine("Press Enter to return...");
        Console.ReadLine();
    }


    private async Task RefreshBudgets()
    {
        var result = await _userController.TryGetUserBudgets(_currentUser);

        if (!result.Success)
        {
            Console.WriteLine("Couldn't Refresh Budgets");
            result.errors.ForEach(Console.WriteLine);
            return;
        }
        
        _budgets = result.Budgets;
    }
}
