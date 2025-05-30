using Microsoft.EntityFrameworkCore;
using Model.Entities;
using Model.Interfaces;
using Model.Utilities;

namespace Model.Repositories
{
    /// <summary>
    /// Repository for handling <see cref="User"/> entities, implementing <see cref="IUserRepository"/>.
    /// Provides data access methods for retrieving, adding, updating, and deleting users, as well as related data such as incomes, expenses, and budgets.
    /// </summary>
    public class UserRepository : GenericRepository<User>, IUserRepository
    {
        /// <inheritdoc/>
        public async Task<User?> RetrieveUserByEmail(string email)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        /// <inheritdoc/>
        public async Task<User?> GetUserById(int id)
        {
            User? user = await base.GetById(id);
            return user;
        }

        /// <inheritdoc/>
        public async Task<bool> CheckUserExistsByEmail(string email)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            return await this.RetrieveUserByEmail(email) != null;
        }

        /// <inheritdoc/>
        public async Task<ICollection<Income>> GetUserIncomes(User user)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var incomes = await _dbContext.Incomes.AsNoTracking().
                Where(i => i.UserId == user.Id).ToListAsync();
            user.Incomes = incomes;
            return incomes;
        }

        /// <inheritdoc/>
        public async Task<ICollection<Budget>> GetUserBudgets(User user)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var budgets = await _dbContext.Budgets.AsNoTracking().
                Where(b => b.UserId == user.Id).ToListAsync();

            user.Budgets = budgets;
            return budgets;
        }

        /// <inheritdoc/>
        public async Task<ICollection<Expense>> GetUserExpenses(User user)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();
            var expenses = await _dbContext.Expenses
                .AsNoTracking()
                .Where(e => e.UserId == user.Id)
                .ToListAsync();

            user.Expenses = expenses;
            return expenses;
        }

        /// <inheritdoc/>
        public async Task<decimal> GetTotalUserIncomes(int userId)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();

            decimal incomes = _dbContext.Incomes.Where(i => i.UserId == userId).Sum(i => i.Amount);

            return incomes;
        }

        /// <inheritdoc/>
        public async Task<decimal> GetTotalUserExpenses(int userId)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();

            decimal expenses = _dbContext.Expenses.Where(e => e.UserId == userId).Sum(e => e.SpentAmount);

            return expenses;
        }

        /// <inheritdoc/>
        public async Task<decimal> GetTotalBudgetSpentAmount(int userId, int budgetId)
        {
            await using var _dbContext = _dbContextFactory.CreateDbContext();

            decimal total = _dbContext.Expenses.Where(e => e.UserId == userId && e.BudgetId == budgetId).Sum(e => e.SpentAmount);

            return total;
        }
    }
}
