using Model.Entities;
using Model.Interfaces;
using Model.Utilities;

namespace Model.Handlers
{
    /// <summary>
    /// Handler class responsible for managing expense-related operations such as adding, deleting, updating, searching, and setting reminders for expenses.
    /// </summary>
    public class ExpenseHandler
    {
        private readonly IExpenseRepository _expenseRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="ExpenseHandler"/> class.
        /// </summary>
        public ExpenseHandler()
        {
            _expenseRepository = ServicesContainer.Instance.GetService<IExpenseRepository>();
        }

        /// <summary>
        /// Adds a new expense record to the repository.
        /// </summary>
        /// <param name="expense">The expense to be added.</param>
        /// <returns>
        /// A <see cref="Task{int}"/> representing the asynchronous operation. The task result is the ID of the added expense.
        /// </returns>
        public async Task<int> AddNewExpense(Expense expense)
        {
            try
            {
                await _expenseRepository.Add(expense);
            }
            catch (Exception e)
            {
                LogError("AddNewExpense", e);
            }

            return expense.Id;
        }

        /// <summary>
        /// Deletes an expense record from the repository.
        /// </summary>
        /// <param name="expense">The expense to be deleted.</param>
        /// <returns>
        /// A <see cref="Task{int}"/> representing the asynchronous operation. The task result is the ID of the deleted expense,
        /// or <c>-1</c> if the expense does not exist.
        /// </returns>
        public async Task<int> DeleteExpense(Expense expense)
        {
            try
            {
                if (!await _expenseRepository.CheckExist(expense.Id))
                    return -1;

                return await _expenseRepository.Delete(expense);
            }
            catch (Exception e)
            {
                LogError("DeleteExpense", e);
                return -1;
            }
        }

        /// <summary>
        /// Searches for an expense record by its name.
        /// </summary>
        /// <param name="userId">The user ID whose expense is being searched.</param>
        /// <param name="expenseName">The name of the expense to search for.</param>
        /// <returns>
        /// A <see cref="Task{Expense?}"/> representing the asynchronous operation. The task result is the expense found, or <c>null</c> 
        /// if no expense with the specified name exists.
        /// </returns>
        public async Task<Expense?> SearchExpenseByName(int userId, string expenseName)
        {
            try
            {
                return await _expenseRepository.GetExpenseByName(userId, expenseName);
            }
            catch (Exception e)
            {
                LogError("SearchExpenseByName", e);
                return null;
            }
        }

        /// <summary>
        /// Retrieves all expenses that have reminders set.
        /// </summary>
        /// <param name="userId">The user ID whose expenses with reminders are being retrieved.</param>
        /// <returns>
        /// A <see cref="Task{List{Expense}}"/> representing the asynchronous operation. The task result is a list of expenses that have reminders set,
        /// or an empty list if no such expenses exist.
        /// </returns>
        public async Task<List<Expense>> GetExpensesThatHasReminders(int userId)
        {
            try
            {
                return await _expenseRepository.GetAllThatHasReminder(userId);
            }
            catch (Exception e)
            {
                LogError("GetExpensesThatHasReminders", e);
                return new List<Expense>();
            }
        }

        /// <summary>
        /// Updates an existing expense record with new values.
        /// </summary>
        /// <param name="newValues">The new values for the expense.</param>
        /// <returns>
        /// A <see cref="Task{Expense?}"/> representing the asynchronous operation. The task result is the updated expense, 
        /// or <c>null</c> if the expense could not be found or updated.
        /// </returns>
        public async Task<Expense?> UpdateExpense(Expense newValues)
        {
            try
            {
                var oldExpense = await _expenseRepository.GetById(newValues.Id)!;
                if (oldExpense == null)
                    return null;

                oldExpense.ExpenseName = newValues.ExpenseName;
                oldExpense.RequiredAmount = newValues.RequiredAmount;
                oldExpense.BudgetId = newValues.BudgetId;
                oldExpense.DateCycle = newValues.DateCycle;
                oldExpense.SpentAmount = newValues.SpentAmount;

                await _expenseRepository.Update(oldExpense);
                return oldExpense;
            }
            catch (Exception e)
            {
                LogError("UpdateExpense", e);
                return null;
            }
        }

        /// <summary>
        /// Sets a reminder for a specific expense.
        /// </summary>
        /// <param name="expense">The expense for which the reminder is being set.</param>
        /// <param name="time">The time when the reminder should be triggered.</param>
        /// <returns>
        /// A <see cref="Task{bool}"/> representing the asynchronous operation. The task result is <c>true</c> if the reminder was successfully set,
        /// or <c>false</c> if there was an error.
        /// </returns>
        public async Task<bool> SetExpenseWithReminder(Expense expense, DateTime time)
        {
            try
            {
                await _expenseRepository.SetReminderTime(expense, time);
                return true;
            }
            catch (Exception e)
            {
                LogError("SetExpensesWithReminder", e);
                return false;
            }
        }

        /// <summary>
        /// Logs errors to the console with context and details.
        /// </summary>
        /// <param name="context">The method or operation where the error occurred.</param>
        /// <param name="e">The exception to log.</param>
        private void LogError(string context, Exception e)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error occurred at {context}");
            Console.ForegroundColor = originalColor;

            Console.WriteLine($"Error: {e.Message}");
            Console.WriteLine($"Stack: {e.StackTrace}");
        }
    }
}
