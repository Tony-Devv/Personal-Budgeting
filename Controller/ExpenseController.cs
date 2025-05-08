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

    public async Task<(bool Success, Expense? newExpense)> TryUpdateExpense
        (Expense newValues)
    {
        var updatedExpense = await _expenseHandler.UpdateExpense(newValues);
        return (updatedExpense != null, updatedExpense);
    }

    public async Task<bool> TryDeleteExpense(Expense expense)
    {
        int result = await _expenseHandler.DeleteExpense(expense);
        return result != -1;
    }

    public async Task<(bool Success,Expense? expense)> TrySearchExpense(string expenseName, int userId)
    {
        var expense = await _expenseHandler.SearchExpenseByName(userId, expenseName);
        return (expense != null, expense);
    }


    public async Task<bool> TrySetExpensesReminders(List<Expense> expenses,DateTime reminderTime)
    {
        bool result = await _expenseHandler.SetExpensesWithReminder(expenses,reminderTime);
        return result;
    }

    public async Task<(bool Success, List<Expense> expenses)> TryGetExpensesWithReminder(int userId)
    {
        var expenses = await _expenseHandler.GetExpensesThatHasReminders(userId);
        return (expenses.Count != 0, expenses);
    }
}


