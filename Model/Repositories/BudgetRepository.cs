using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Model.Interfaces;

namespace Model.Repositories;

/// <summary>
/// Repository class responsible for performing CRUD operations on <see cref="Budget"/> entities.
/// Inherits from <see cref="GenericRepository{Budget}"/> and implements <see cref="IBudgetRepository"/>.
/// </summary>
public class BudgetRepository : GenericRepository<Budget> , IBudgetRepository
{
    
    /// <inheritdoc cref="IBudgetRepository"/>
    public async Task<Budget?> GetBudgetByName(int userId, string budgetName)
    {
        await using var _dbContext = _dbContextFactory.CreateDbContext();
        return await _dbContext.Budgets.
            Where(b => b.UserId == userId && b.BudgetName == budgetName).FirstOrDefaultAsync();
    }
}