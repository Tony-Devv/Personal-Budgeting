using Model.Entities;
using Model.Interfaces;
using Model.Utilities;

namespace Model.Handlers;

public class IncomeHandler
{
    private readonly IIncomeRepository _incomeRepository;

    public IncomeHandler()
    {
        _incomeRepository = ServicesContainer.Instance.GetService<IIncomeRepository>();
    }

    public async Task<int> AddNewIncome(Income income)
    {
        try
        {
            await _incomeRepository.Add(income);
        }
        catch (Exception e)
        {
            LogError("AddNewIncome", e);
        }

        return income.Id;
    }

    public async Task<int> DeleteIncome(Income income)
    {
        try
        {
            if (!await _incomeRepository.CheckExist(income.Id))
                return -1;

            return await _incomeRepository.Delete(income);
        }
        catch (Exception e)
        {
            LogError("DeleteIncome", e);
            return -1;
        }
    }

    public async Task<Income?> SearchIncomeBySourceName(int userId, string incomeSource)
    {
        try
        {
            return await _incomeRepository.GetIncomeBySourceName(userId, incomeSource);
        }
        catch (Exception e)
        {
            LogError("SearchIncomeBySourceName", e);
            return null;
        }
    }

    public async Task<Income?> UpdateIncome(Income newValues)
    {
        try
        {
            var oldIncome = await _incomeRepository.GetById(newValues.Id)!;
            if (oldIncome == null)
                return null;

            oldIncome.IncomeSourceName = newValues.IncomeSourceName;
            oldIncome.Amount = newValues.Amount;
            oldIncome.IncomeDate = newValues.IncomeDate;

            await _incomeRepository.Update(oldIncome);
            return oldIncome;
        }
        catch (Exception e)
        {
            LogError("UpdateIncome", e);
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
