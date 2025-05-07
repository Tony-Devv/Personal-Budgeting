using Model.Entities;

namespace Model.Interfaces;

public interface IBudgetRepository : IRepository<Budget>
{
    Task<Budget?> GetBudgetByName(int userId, string budgetName);
}