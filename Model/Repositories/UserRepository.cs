using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Model.Interfaces;
using Model.Utilities;

namespace Model.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    
    public async Task<User?> RetrieveUserByEmail(string email)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> CheckUserExistsByEmail(string email)
    {
        return await this.RetrieveUserByEmail(email) != null;
    }

    public async Task<ICollection<Income>> GetUserIncomes(User user)
    {
        var incomes = await _dbContext.Incomes.AsNoTracking().
            Where(i => i.UserId == user.Id).ToListAsync();
        user.Incomes = incomes;
        return incomes;
    }

    public async Task<ICollection<Budget>> GetUserBudgets(User user)
    {
        var budgets = await _dbContext.Budgets.AsNoTracking().
            Where(b => b.UserId == user.Id).ToListAsync();

        user.Budgets = budgets;
        return budgets;
    }

    public async Task<ICollection<Expense>> GetUserExpenses(User user)
    {
        var expenses = await _dbContext.Expenses
            .AsNoTracking()
            .Where(e => e.UserId == user.Id)
            .ToListAsync();

        user.Expenses = expenses;
        return expenses;
    }
}