using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Model.Interfaces;

namespace Model.Repositories;

public class ExpenseRepository : GenericRepository<Expense>, IExpenseRepository
{
    public async Task SetReminderTime(Expense expense, DateTime time)
    {
        if (! await base.CheckExist(expense.Id))
        {
            return;
        }
        
        var e = await _dbContext.Expenses.FindAsync(expense);
        if (e != null)
        {
            e.ReminderTime = time; 
            _dbContext.Entry(expense).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        } 
    }

    public async Task<Expense?> GetExpenseByName(int userId, string expenseName)
    {
        return await _dbContext.Expenses
            .Where(e => e.UserId == userId && e.ExpenseName == expenseName)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Expense>> GetAllThatHasReminder(int userId)
    {
        return await _dbContext.Expenses
            .Where(e => e.UserId == userId && e.ReminderTime != null)
            .ToListAsync();
    }
}