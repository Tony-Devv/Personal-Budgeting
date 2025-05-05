using Model.Entities;
using Model.Interfaces;

namespace Model.Repositories;

public class IncomeRepository : GenericRepository<Income>, IIncomeRepository
{
    public Task<Income> GetIncomeBySourceName(int userId, string sourceName)
    {
        throw new NotImplementedException();
    }
}