using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Model.Interfaces;
using Model.Utilities;

namespace Model.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    
    public async Task<User?> RetrieveUserByEmail(string email)
    {
        await using var _dbContext = _dbContextFactory.CreateDbContext();
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> CheckUserExistsByEmail(string email)
    {
        await using var _dbContext = _dbContextFactory.CreateDbContext();
        return await this.RetrieveUserByEmail(email) != null;
    }

    public async Task<ICollection<Income>> GetUserIncomes(User user)
    {
        await using var _dbContext = _dbContextFactory.CreateDbContext();
        var incomes = await _dbContext.Incomes.AsNoTracking().
            Where(i => i.UserId == user.Id).ToListAsync();
        user.Incomes = incomes;
        return incomes;
    }

    public async Task<ICollection<Budget>> GetUserBudgets(User user)
    {
        await using var _dbContext = _dbContextFactory.CreateDbContext();
        var budgets = await _dbContext.Budgets.AsNoTracking().
            Where(b => b.UserId == user.Id).ToListAsync();

        user.Budgets = budgets;
        return budgets;
    }

    public async Task<ICollection<Expense>> GetUserExpenses(User user)
    {
        await using var _dbContext = _dbContextFactory.CreateDbContext();
        var expenses = await _dbContext.Expenses
            .AsNoTracking()
            .Where(e => e.UserId == user.Id)
            .ToListAsync();

        user.Expenses = expenses;
        return expenses;
    }
}