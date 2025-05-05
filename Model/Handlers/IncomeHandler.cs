using Model.Entities;
using Model.Interfaces;

namespace Model.Handlers;

public class IncomeHandler
{
    private readonly IIncomeRepository _incomeRepository;


    public async Task<int> AddNewIncome(Income income)
    {
        return -1;
    }

    public async Task<int> DeleteIncome(Income income)
    {
        return -1;
    }

    public async Task<Income> SearchIncomeBySourceName(int userId, string IncomeSource)
    {
        return new Income();
    }

    public async Task<Income> GetIncomeById(int id)
    {
        return new Income();
    }

    public async Task<Income> UpdateIncome(int incomeId, Income newValues)
    {
        return new Income();
    }
}