using Model.Entities;

namespace Model.Interfaces;

public interface IExpenseRepository : IRepository<Expense>
{
    Task<bool> SetReminderTime(Expense expense, DateTime time);
    Task<Expense?> GetExpenseByName(int userId, string expenseName);
    Task<List<Expense>> GetAllThatHasReminder(int userId);
}