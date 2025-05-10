using Model.Entities;
using Model.Interfaces;
using Model.Utilities;

namespace Model.Handlers;

public class ExpenseHandler
{
    private readonly IExpenseRepository _expenseRepository;

    public ExpenseHandler()
    {
        _expenseRepository = ServicesContainer.Instance.GetService<IExpenseRepository>();
    }

    public async Task<int> AddNewExpense(Expense expense)
    {
        try
        {
            await _expenseRepository.Add(expense);
        }
        catch (Exception e)
        {
            LogError("AddNewExpense", e);
        }

        return expense.Id;
    }

    public async Task<int> DeleteExpense(Expense expense)
    {
        try
        {
            if (!await _expenseRepository.CheckExist(expense.Id))
                return -1;

            return await _expenseRepository.Delete(expense);
        }
        catch (Exception e)
        {
            LogError("DeleteExpense", e);
            return -1;
        }
    }

    public async Task<Expense?> SearchExpenseByName(int userId, string expenseName)
    {
        try
        {
            return await _expenseRepository.GetExpenseByName(userId, expenseName);
        }
        catch (Exception e)
        {
            LogError("SearchExpenseByName", e);
            return null;
        }
    }

    public async Task<List<Expense>> GetExpensesThatHasReminders(int userId)
    {
        try
        {
            return await _expenseRepository.GetAllThatHasReminder(userId);
        }
        catch (Exception e)
        {
            LogError("GetExpensesThatHasReminders", e);
            return new List<Expense>();
        }
    }

    public async Task<Expense?> UpdateExpense(Expense newValues)
    {
        try
        {
            var oldExpense = await _expenseRepository.GetById(newValues.Id)!;
            if (oldExpense == null)
                return null;

            oldExpense.ExpenseName = newValues.ExpenseName;
            oldExpense.RequiredAmount = newValues.RequiredAmount;
            oldExpense.BudgetId = newValues.BudgetId;
            oldExpense.DateCycle = newValues.DateCycle;
            oldExpense.SpentAmount = newValues.SpentAmount;

            await _expenseRepository.Update(oldExpense);
            return oldExpense;
        }
        catch (Exception e)
        {
            LogError("UpdateExpense", e);
            return null;
        }
    }

    public async Task<bool> SetExpensesWithReminder(List<Expense> expenses, DateTime time)
    {
        try
        {
            foreach (var expense in expenses)
            {
                if (expense.ReminderTime == null)
                {
                    await _expenseRepository.SetReminderTime(expense, time);
                }
            }

            return true;
        }
        catch (Exception e)
        {
            LogError("SetExpensesWithReminder", e);
            return false;
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
