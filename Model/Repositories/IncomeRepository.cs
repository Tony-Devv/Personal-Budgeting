using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Model.Interfaces;
using Model.Utilities;

namespace Model.Repositories;

public class IncomeRepository : GenericRepository<Income>, IIncomeRepository
{
    
    public async Task<Income?> GetIncomeBySourceName(int userId, string sourceName)
    {
        return await _dbContext.Incomes.Where(i => i.UserId == userId && i.SourceName == sourceName).FirstOrDefaultAsync();
    }
}