using Model.Entities;
using Model.Handlers;
using Model.Utilities;

public class IncomeController
{
    private readonly IncomeHandler _incomeHandler = ServicesContainer.Instance.GetService<IncomeHandler>();

    public async Task<(bool Success, int IncomeId)> TryAddIncome(Income income)
    {
        int result = await _incomeHandler.AddNewIncome(income);
        return (result != -1, result);
    }

    public async Task<(bool Success, int IncomeId)> TryUpdateIncome(int incomeId, Income newValues)
    {
        var updatedIncome = await _incomeHandler.UpdateIncome(incomeId, newValues);
        return (updatedIncome != null, updatedIncome);
    }

    public async Task<bool> TryDeleteIncome(Income income)
    {
        int result = await _incomeHandler.DeleteIncome(income);
        return (result != -1,result);
    }

    public async Task<bool> TrySearchIncome(string sourceName, int userId)
    {
        var income = await _incomeHandler.SearchIncomeBySourceName(userId, sourceName);
        return (income != null,income);
    }

    public async Task<bool> TryGetIncomeById(int id)
    {
        var income = await _incomeHandler.GetIncomeById(id);
        return (income != null,income);
    }
}
