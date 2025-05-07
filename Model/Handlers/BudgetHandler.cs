using Model.Entities;
using Model.Interfaces;
using Model.Utilities;

namespace Model.Handlers;

public class BudgetHandler
{
    private readonly IBudgetRepository _budgetRepository;


    public BudgetHandler()
    {
        _budgetRepository = ServicesContainer.Instance.GetService<IBudgetRepository>();
    }
    
    public async Task<int> AddNewBudget(Budget budget)
    {

        return -1;
    }

    public async Task<int> DeleteBudget(Budget budget)
    {

        return -1;
    }

    public async Task<Budget> GetBudgetById(int id)
    {
        return new Budget();
    }
    
    public async Task<Budget> GetBudgetByName(int userId, string budgetName)
    {
        return new Budget();
    }

    public async Task<Budget> UpdateBudget(int budgetId, Budget newValues)
    {
        return new Budget();
    }
}