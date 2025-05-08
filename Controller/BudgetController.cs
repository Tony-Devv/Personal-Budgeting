using Model.Entities;
using Model.Handlers;
using Model.Utilities;

public class BudgetController
{
    private readonly BudgetHandler _budgetHandler = 
        ServicesContainer.Instance.GetService<BudgetHandler>();

    public async Task<(bool Success, int BudgetId)> TryAddBudget(Budget budget)
    {
        int result = await _budgetHandler.AddNewBudget(budget);
        return (result != -1, result);
    }

    public async Task<(bool Success, Budget? BudgetId)> TryUpdateBudget(Budget newValues)
    {
        var updatedBudget = await _budgetHandler.UpdateBudget(newValues);
        return (updatedBudget != null, updatedBudget);
    }

    public async Task<bool> TryDeleteBudget(Budget budget)
    {
        int result = await _budgetHandler.DeleteBudget(budget);
        return result != -1;
    }

    public async Task<bool> TryGetBudgetByName(int userId, string budgetName)
    {
        var budget = await _budgetHandler.GetBudgetByName(userId, budgetName);
        return budget != null;
    }
}
