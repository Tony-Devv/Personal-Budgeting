using Model.Entities;

namespace Model.Interfaces
{
    /// <summary>
    /// Provides data access methods specific to incomes, extending the generic repository for <see cref="Income"/>.
    /// </summary>
    public interface IIncomeRepository : IRepository<Income>
    {
        /// <summary>
        /// Retrieves an income by its source name for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user who owns the income.</param>
        /// <param name="sourceName">The name of the income source to retrieve.</param>
        /// <returns>A task that returns the <see cref="Income"/> if found; otherwise, null.</returns>
        Task<Income?> GetIncomeBySourceName(int userId, string sourceName);
    }
}