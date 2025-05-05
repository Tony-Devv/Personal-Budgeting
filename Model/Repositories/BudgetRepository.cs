using Model.Entities;
using Model.Interfaces;

namespace Model.Repositories;

public class BudgetRepository : GenericRepository<Budget> , IBudgetRepository
{
    public Task<Budget> GetBudgetByName(int userId, string budgetName)
    {
        throw new NotImplementedException();
    }
}