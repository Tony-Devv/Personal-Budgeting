using Model.Entities;

namespace Model.Interfaces
{
    /// <summary>
    /// Provides data access methods specific to expenses, extending the generic repository for <see cref="Expense"/>.
    /// </summary>
    public interface IExpenseRepository : IRepository<Expense>
    {
        /// <summary>
        /// Sets a reminder time for a specific expense.
        /// </summary>
        /// <param name="expense">The expense to set the reminder for.</param>
        /// <param name="time">The time at which the reminder should trigger.</param>
        /// <returns>A task that returns true if the reminder was successfully set; otherwise, false.</returns>
        Task<bool> SetReminderTime(Expense expense, DateTime time);

        /// <summary>
        /// Retrieves an expense by its name for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user who owns the expense.</param>
        /// <param name="expenseName">The name of the expense to retrieve.</param>
        /// <returns>A task that returns the <see cref="Expense"/> if found; otherwise, null.</returns>
        Task<Expense?> GetExpenseByName(int userId, string expenseName);

        /// <summary>
        /// Retrieves all expenses that have an active reminder set for a specific user.
        /// </summary>
        /// <param name="userId">The ID of the user whose expenses to retrieve.</param>
        /// <returns>A task that returns a list of expenses with reminders set.</returns>
        Task<List<Expense>> GetAllThatHasReminder(int userId);
    }
}