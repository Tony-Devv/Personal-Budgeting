using Model.Entities;
using Model.Handlers;
using Model.Utilities;

public class ExpenseController
{
    private readonly ExpenseHandler _expenseHandler = ServicesContainer.Instance.GetService<ExpenseHandler>();

    public async Task<(bool Success, int ExpenseId)> TryAddExpense(Expense expense)
    {
        int result = await _expenseHandler.AddNewExpense(expense);
        return (result != -1, result);
    }

    public async Task<(bool Success, int ExpenseId)> TryUpdateExpense(int expenseId, Expense newValues)
    {
        var updatedExpense = await _expenseHandler.UpdateExpense(expenseId, newValues);
        return (updatedExpense != null, updatedExpense);
    }

    public async Task<bool> TryDeleteExpense(Expense expense)
    {
        int result = await _expenseHandler.DeleteExpense(expense);
        return result != -1;
    }

    public async Task<bool> TrySearchExpense(string expenseName, int userId)
    {
        var expense = await _expenseHandler.SearchExpenseByName(userId, expenseName);
        return expense != null;
    }


    public async Task<bool> TrySetExpenseReminder(int expenseId, DateTime reminderTime)
    {
        var Reminder = await _expenseHandler.GetExpensesThatHasReminders(expenseId);
        return Reminder!=null;   
    }
}


