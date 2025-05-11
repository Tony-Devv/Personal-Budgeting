using Model.Entities;
using Model.Interfaces;
using Model.Utilities;

namespace Model.Handlers
{
    /// <summary>
    /// Handler class responsible for managing budget-related operations such as adding, deleting, updating, and retrieving budgets.
    /// </summary>
    public class BudgetHandler
    {
        private readonly IBudgetRepository _budgetRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="BudgetHandler"/> class.
        /// </summary>
        public BudgetHandler()
        {
            _budgetRepository = ServicesContainer.Instance.GetService<IBudgetRepository>();
        }

        /// <summary>
        /// Adds a new budget record to the repository.
        /// </summary>
        /// <param name="budget">The budget to be added.</param>
        /// <returns>
        /// A <see cref="Task{int}"/> representing the asynchronous operation. The task result is the ID of the added budget.
        /// </returns>
        public async Task<int> AddNewBudget(Budget budget)
        {
            try
            {
                await _budgetRepository.Add(budget);
            }
            catch (Exception e)
            {
                LogError("AddNewBudget", e);
            }

            return budget.Id;
        }

        /// <summary>
        /// Deletes a budget record from the repository.
        /// </summary>
        /// <param name="budget">The budget to be deleted.</param>
        /// <returns>
        /// A <see cref="Task{int}"/> representing the asynchronous operation. The task result is the ID of the deleted budget,
        /// or <c>-1</c> if the budget does not exist.
        /// </returns>
        public async Task<int> DeleteBudget(Budget budget)
        {
            try
            {
                if (!await _budgetRepository.CheckExist(budget.Id))
                    return -1;

                return await _budgetRepository.Delete(budget);
            }
            catch (Exception e)
            {
                LogError("DeleteBudget", e);
                return -1;
            }
        }

        /// <summary>
        /// Retrieves a budget record by its name.
        /// </summary>
        /// <param name="userId">The user ID whose budget is being retrieved.</param>
        /// <param name="budgetName">The name of the budget to retrieve.</param>
        /// <returns>
        /// A <see cref="Task{Budget?}"/> representing the asynchronous operation. The task result is the budget found, or <c>null</c> 
        /// if no budget with the specified name exists.
        /// </returns>
        public async Task<Budget?> GetBudgetByName(int userId, string budgetName)
        {
            try
            {
                return await _budgetRepository.GetBudgetByName(userId, budgetName);
            }
            catch (Exception e)
            {
                LogError("GetBudgetByName", e);
                return null;
            }
        }

        /// <summary>
        /// Updates an existing budget record with new values.
        /// </summary>
        /// <param name="newValues">The new values for the budget.</param>
        /// <returns>
        /// A <see cref="Task{Budget?}"/> representing the asynchronous operation. The task result is the updated budget, 
        /// or <c>null</c> if the budget could not be found or updated.
        /// </returns>
        public async Task<Budget?> UpdateBudget(Budget newValues)
        {
            try
            {
                var oldBudget = await _budgetRepository.GetById(newValues.Id);
                if (oldBudget == null)
                    return null;

                oldBudget.BudgetName = newValues.BudgetName;
                oldBudget.TotalAmountRequired = newValues.TotalAmountRequired;

                await _budgetRepository.Update(oldBudget);
                return oldBudget;
            }
            catch (Exception e)
            {
                LogError("UpdateBudget", e);
                return null;
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
