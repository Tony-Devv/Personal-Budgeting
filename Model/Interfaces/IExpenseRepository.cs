using Model.Entities;

namespace Model.Interfaces;

public interface IExpenseRepository : IRepository<Expense>
{
    Task SetReminderTime(Expense expense, DateTime time);
    Task<Expense?> GetExpenseByName(int userId, string expenseName);
    Task<List<Expense>> GetAllThatHasReminder(int userId);
}