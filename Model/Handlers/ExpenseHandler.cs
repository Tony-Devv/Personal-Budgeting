using Model.Entities;
using Model.Interfaces;

namespace Model.Handlers;

public class ExpenseHandler
{
    private readonly IExpenseRepository _expenseRepository;

    public async Task<int> AddNewExpense(Expense expense)
    {
        return -1;
    }

    public async Task<int> DeleteExpense(Expense expense)
    {
        return -1;
    }

    public async Task<Expense> SearchExpenseByName(int userId, string expenseName)
    {

        return new Expense();
    }

    public async Task<List<Expense>> GetExpensesThatHasReminders(int userId)
    {
        return new List<Expense>();
    }

    public async Task<Expense> GetExpenseById(int id)
    {
        return new Expense();
    }
    
    public async Task<Expense> UpdateExpense(int expenseId, Expense newValues)
    {
        return new Expense();
    }
    
    public async void SetExpensesWithReminder(List<Expense> expenses, DateTime time)
    {
        
    }
}