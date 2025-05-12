using Model.Entities;

namespace View.Views;

public class IncomeView : IView
{
    private IncomeController _incomeController;
    private UserController _userController;
    private User _currentUser;
    private List<Income> _incomes;

    public async Task Initialize(User obj)
    {
        _incomeController = new IncomeController();
        _userController = new UserController();
        _currentUser = obj;
        _incomes = new List<Income>();
    }

    public async Task Show()
    {
        await RefreshIncomes();
        while (true)
        {
            Console.Clear();
            Console.WriteLine("== Income Dashboard ==");
            Console.WriteLine("1 - Show Incomes");
            Console.WriteLine("2 - Add New Income");
            Console.WriteLine("3 - Edit Certain Income");
            Console.WriteLine("4 - Delete Income");
            Console.WriteLine("5 - Back");
            Console.Write("Enter choice: ");
            string input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    await ShowIncomes();
                    break;
                case "2":
                    await AddNewIncome();
                    break;
                case "3":
                    await EditIncome();
                    break;
                case "4":
                    await DeleteIncome();
                    break;
                case "5":
                    return;
                default:
                    Console.WriteLine("Invalid option. Press Enter to continue.");
                    Console.ReadLine();
                    break;
            }
        }
    }

    private async Task ShowIncomes()
    {
        var (success, incomes, errors) = await _userController.TryGetUserIncomes(_currentUser);
        if (!success)
        {
            Console.WriteLine("Failed to retrieve incomes:");
            errors.ForEach(e => Console.WriteLine($"- {e}"));
        }
        else if (incomes.Count == 0)
        {
            Console.WriteLine("No incomes found.");
        }
        else
        {
            _incomes = incomes;
            Console.WriteLine("Incomes:");
            foreach (var income in _incomes)
                Console.WriteLine(income);
        }

        Console.WriteLine("Press Enter to return to menu...");
        Console.ReadLine();
    }

    private async Task AddNewIncome()
    {
        var income = new Income();
        income.UserId = _currentUser.Id;

        Console.Write("Enter income source name: ");
        income.IncomeSourceName = Console.ReadLine();

        Console.Write("Enter amount: ");
        if (!decimal.TryParse(Console.ReadLine(), out var amount))
        {
            Console.WriteLine("Invalid amount.");
            Console.ReadLine();
            return;
        }
        income.Amount = amount;
        income.IncomeDate = DateTime.Now;

        var (success, id, errors) = await _incomeController.TryAddIncome(income,true);
        if (!success)
        {
            Console.WriteLine("Failed to add income:");
            errors.ForEach(e => Console.WriteLine($"- {e}"));
        }
        else
        {
            Console.WriteLine($"Income added successfully with ID: {id}");
        }

        Console.WriteLine("Press Enter to return...");
        Console.ReadLine();
        await RefreshIncomes();
    }

    private async Task EditIncome()
    {
        Console.Write("Enter the income source name to edit: ");
        var sourceName = Console.ReadLine();

        var target = _incomes.FirstOrDefault(i => i.IncomeSourceName.Equals(sourceName, StringComparison.OrdinalIgnoreCase));
        if (target == null)
        {
            Console.WriteLine("Income not found.");
            Console.ReadLine();
            return;
        }

        Console.Write("Enter new source name (leave blank to keep current): ");
        string newName = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newName))
            target.IncomeSourceName = newName;

        Console.Write("Enter new amount (leave blank to keep current): ");
        var amountInput = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(amountInput) && decimal.TryParse(amountInput, out var newAmount))
            target.Amount = newAmount;

        var (success, updated, errors) = await _incomeController.TryUpdateIncome(target,true);
        if (!success)
        {
            Console.WriteLine("Failed to update income:");
            errors.ForEach(e => Console.WriteLine($"- {e}"));
        }
        else
        {
            Console.WriteLine("Income updated successfully.");
        }

        Console.WriteLine("Press Enter to return...");
        Console.ReadLine();
    }

    private async Task DeleteIncome()
    {
        Console.Write("Enter the income source name to delete: ");
        var sourceName = Console.ReadLine();

        var target = _incomes.FirstOrDefault(i => i.IncomeSourceName.Equals(sourceName, StringComparison.OrdinalIgnoreCase));
        if (target == null)
        {
            Console.WriteLine("Income not found.");
            Console.ReadLine();
            return;
        }

        var (success, errors) = await _incomeController.TryDeleteIncome(target,true);
        if (!success)
        {
            Console.WriteLine("Failed to delete income:");
            errors.ForEach(e => Console.WriteLine($"- {e}"));
        }
        else
        {
            Console.WriteLine("Income deleted successfully.");
            _incomes.Remove(target);
        }

        Console.WriteLine("Press Enter to return...");
        Console.ReadLine();
    }
    
    private async Task RefreshIncomes()
    {
        var result = await _userController.TryGetUserIncomes(_currentUser);

        if (!result.Success)
        {
            Console.WriteLine("Couldn't refresh incomes.");
            result.errors.ForEach(Console.WriteLine);
            return;
        }

        _incomes = result.Incomes;
    }

}
