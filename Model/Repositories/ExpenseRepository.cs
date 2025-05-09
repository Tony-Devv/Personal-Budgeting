using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Model.Interfaces;

namespace Model.Repositories;

public class ExpenseRepository : GenericRepository<Expense>, IExpenseRepository
{
    public async Task<bool> SetReminderTime(Expense expense, DateTime time)
    {
        await using var _dbContext = _dbContextFactory.CreateDbContext();
        if (! await base.CheckExist(expense.Id))
        {
            return false;
        }
        
        var E = await _dbContext.Expenses.FirstOrDefaultAsync(e => e.Id == expense.Id 
                                                                   && e.UserId == expense.UserId);
        if (E != null)
        {
            E.ReminderTime = time;
            _dbContext.Expenses.Update(E);
            await _dbContext.SaveChangesAsync();
            await _dbContext.Expenses.Entry(E).ReloadAsync(); // refresh the entity in the memory
            return true;
        }

        return false;
    }

    public async Task<Expense?> GetExpenseByName(int userId, string expenseName)
    {
        await using var _dbContext = _dbContextFactory.CreateDbContext();
        return await _dbContext.Expenses
            .Where(e => e.UserId == userId && e.ExpenseName == expenseName)
            .FirstOrDefaultAsync();
    }

    public async Task<List<Expense>> GetAllThatHasReminder(int userId)
    {
        await using var _dbContext = _dbContextFactory.CreateDbContext();
        return await _dbContext.Expenses
            .Where(e => e.UserId == userId && e.ReminderTime != null)
            .ToListAsync();
    }
}