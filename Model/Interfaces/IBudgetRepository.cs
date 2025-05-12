using Model.Entities;

namespace Model.Interfaces
{
    /// <summary>
    /// Provides data access methods specific to budgets, extending the generic repository for <see cref="Budget"/>.
    /// </summary>
    public interface IBudgetRepository : IRepository<Budget>
    {
        /// <summary>
        /// Retrieves a budget by its name for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user who owns the budget.</param>
        /// <param name="budgetName">The name of the budget to retrieve.</param>
        /// <returns>A task that returns the <see cref="Budget"/> if found; otherwise, null.</returns>
        Task<Budget?> GetBudgetByName(int userId, string budgetName);
    }
}