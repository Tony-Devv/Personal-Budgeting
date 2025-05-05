using Model.Entities;

public class ExpenseController
{
	public bool TryAddNewExpense(Expense expense) { return true; }
	public bool TryDeleteExpense(Expense expense) { return true; }
	public bool TrySearchExpense(string expense, int userId) { return true; }
	public bool TryGetExpensesWithReminders(int userId) { return true; }
	public bool TrySetExpenseWithTimeReminder(Expense expense, DateTime time) { return true; }
}
