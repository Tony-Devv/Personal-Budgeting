using Model.Entities;
using Model.Interfaces;

namespace Model.Repositories;

public class ExpenseRepository : GenericRepository<Expense>, IExpenseRepository
{
    public void SetReminderTime(Expense expense, DateTime time)
    {
        throw new NotImplementedException();
    }

    public Task<Expense> GetExpenseByName(int userId, string expenseName)
    {
        throw new NotImplementedException();
    }

    public Task<List<Expense>> GetAllThatHasReminder(int userId)
    {
        throw new NotImplementedException();
    }
}