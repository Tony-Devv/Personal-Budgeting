using Model.Entities;

namespace Model.Interfaces;

public interface IIncomeRepository : IRepository<Income>
{
    Task<Income?> GetIncomeBySourceName(int userId, string sourceName);
}