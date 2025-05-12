using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Model.Interfaces;
using Model.Utilities;

namespace Model.Repositories
{
    /// <summary>
    /// Repository class responsible for performing CRUD operations on <see cref="Income"/> entities.
    /// Inherits from <see cref="GenericRepository{Income}"/> and implements <see cref="IIncomeRepository"/>.
    /// </summary>
    public class IncomeRepository : GenericRepository<Income>, IIncomeRepository
    {
        /// <inheritdoc />
        public async Task<Income?> GetIncomeBySourceName(int userId, string sourceName)
        {
            // Create a new instance of the DbContext
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            
            // Retrieve the first income record matching the userId and sourceName
            return await _dbContext.Incomes
                .Where(i => i.UserId == userId && i.IncomeSourceName == sourceName)
                .FirstOrDefaultAsync();
        }
    }
}