using Model.Entities;

namespace Model.Interfaces
{
    /// <summary>
    /// Provides data access methods specific to user-related operations, extending the generic repository for <see cref="User"/>.
    /// </summary>
    public interface IUserRepository : IRepository<User>
    {
        /// <summary>
        /// Retrieves a user entity by their email address.
        /// </summary>
        /// <param name="email">The email address of the user.</param>
        /// <returns>A task that returns the <see cref="User"/> if found; otherwise, null.</returns>
        Task<User?> RetrieveUserByEmail(string email);

        /// <summary>
        /// Retrieves a user by their unique identifier.
        /// </summary>
        /// <param name="id">The ID of the user.</param>
        /// <returns>A task that returns the <see cref="User"/> if found; otherwise, null.</returns>
        Task<User?> GetUserById(int id);

        /// <summary>
        /// Checks whether a user exists with the specified email address.
        /// </summary>
        /// <param name="email">The email address to check.</param>
        /// <returns>A task that returns true if the user exists; otherwise, false.</returns>
        Task<bool> CheckUserExistsByEmail(string email);

        /// <summary>
        /// Retrieves the list of incomes associated with a given user.
        /// </summary>
        /// <param name="user">The user whose incomes to retrieve.</param>
        /// <returns>A task that returns a collection of the user's incomes.</returns>
        Task<ICollection<Income>> GetUserIncomes(User user);

        /// <summary>
        /// Retrieves the list of budgets associated with a given user.
        /// </summary>
        /// <param name="user">The user whose budgets to retrieve.</param>
        /// <returns>A task that returns a collection of the user's budgets.</returns>
        Task<ICollection<Budget>> GetUserBudgets(User user);

        /// <summary>
        /// Retrieves the list of expenses associated with a given user.
        /// </summary>
        /// <param name="user">The user whose expenses to retrieve.</param>
        /// <returns>A task that returns a collection of the user's expenses.</returns>
        Task<ICollection<Expense>> GetUserExpenses(User user);

        /// <summary>
        /// Calculates the total income amount for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A task that returns the total income amount.</returns>
        Task<decimal> GetTotalUserIncomes(int userId);

        /// <summary>
        /// Calculates the total expenses amount for the specified user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <returns>A task that returns the total expense amount.</returns>
        Task<decimal> GetTotalUserExpenses(int userId);

        /// <summary>
        /// Retrieves the total spent amount from a specific budget for a user.
        /// </summary>
        /// <param name="userId">The ID of the user.</param>
        /// <param name="budgetId">The ID of the budget.</param>
        /// <returns>A task that returns the total spent amount from the specified budget.</returns>
        Task<decimal> GetTotalBudgetSpentAmount(int userId, int budgetId);
    }
}
