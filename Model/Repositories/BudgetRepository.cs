using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Model.Interfaces;

namespace Model.Repositories;

public class BudgetRepository : GenericRepository<Budget> , IBudgetRepository
{
    public async Task<Budget?> GetBudgetByName(int userId, string budgetName)
    {
        await using var _dbContext = _dbContextFactory.CreateDbContext();
        return await _dbContext.Budgets.
            Where(b => b.UserId == userId && b.BudgetName == budgetName).FirstOrDefaultAsync();
    }
}