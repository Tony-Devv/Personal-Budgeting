using Model.Entities;
using Model.Interfaces;
using Model.Utilities;

namespace Model.Handlers;

public class BudgetHandler
{
    private readonly IBudgetRepository _budgetRepository;

    public BudgetHandler()
    {
        _budgetRepository = ServicesContainer.Instance.GetService<IBudgetRepository>();
    }

    public async Task<int> AddNewBudget(Budget budget)
    {
        try
        {
            await _budgetRepository.Add(budget);
        }
        catch (Exception e)
        {
            LogError("AddNewBudget", e);
        }

        return budget.Id;
    }

    public async Task<int> DeleteBudget(Budget budget)
    {
        try
        {
            if (!await _budgetRepository.CheckExist(budget.Id))
                return -1;

            return await _budgetRepository.Delete(budget);
        }
        catch (Exception e)
        {
            LogError("DeleteBudget", e);
            return -1;
        }
    }

    public async Task<Budget?> GetBudgetByName(int userId, string budgetName)
    {
        try
        {
            return await _budgetRepository.GetBudgetByName(userId, budgetName);
        }
        catch (Exception e)
        {
            LogError("GetBudgetByName", e);
            return null;
        }
    }

    public async Task<Budget?> UpdateBudget(Budget newValues)
    {
        try
        {
            var oldBudget = await _budgetRepository.GetById(newValues.Id);
            if (oldBudget == null)
                return null;

            oldBudget.BudgetName = newValues.BudgetName;
            oldBudget.TotalAmountRequired = newValues.TotalAmountRequired;

            await _budgetRepository.Update(oldBudget);
            return oldBudget;
        }
        catch (Exception e)
        {
            LogError("UpdateBudget", e);
            return null;
        }
    }

    private void LogError(string context, Exception e)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error occurred at {context}");
        Console.ForegroundColor = originalColor;
        Console.WriteLine($"Error: {e.Message}");
        Console.WriteLine($"Stack: {e.StackTrace}");
    }
}
