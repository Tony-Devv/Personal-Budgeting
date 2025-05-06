using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Model.Interfaces;
using Model.Utilities;

namespace Model.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    
    public async Task<User?> RetrieveUserByEmail(string email)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<bool> CheckUserExistsByEmail(string email)
    {
        return await this.RetrieveUserByEmail(email) != null;
    }

    public async Task<ICollection<Income>> GetUserIncomes(User user)
    {
        await _dbContext.Entry(user).Collection(u => u.Incomes).LoadAsync();

        return user.Incomes.ToList();
    }

    public async Task<ICollection<Budget>> GetUserBudgets(User user)
    {
        await _dbContext.Entry(user).Collection(u => u.Budgets).LoadAsync();

        return user.Budgets.ToList();
    }

    public async Task<ICollection<Expense>> GetUserExpenses(User user)
    {
        await _dbContext.Entry(user).Collection(u => u.Expenses).LoadAsync();

        return user.Expenses.ToList();
    }
}