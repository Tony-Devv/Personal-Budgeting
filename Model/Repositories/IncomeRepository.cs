using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Model.Interfaces;
using Model.Utilities;

namespace Model.Repositories;

public class IncomeRepository : GenericRepository<Income>, IIncomeRepository
{
    
    public async Task<Income?> GetIncomeBySourceName(int userId, string sourceName)
    {
        await using var _dbContext = _dbContextFactory.CreateDbContext();
        return await _dbContext.Incomes.Where(i => i.UserId == userId && i.IncomeSourceName == sourceName).FirstOrDefaultAsync();
    }
}