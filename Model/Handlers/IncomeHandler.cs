using Model.Entities;
using Model.Interfaces;
using Model.Utilities;

namespace Model.Handlers
{
    /// <summary>
    /// Handler class responsible for managing income-related operations such as adding, deleting, updating, and searching incomes.
    /// </summary>
    public class IncomeHandler
    {
        private readonly IIncomeRepository _incomeRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="IncomeHandler"/> class.
        /// </summary>
        public IncomeHandler()
        {
            _incomeRepository = ServicesContainer.Instance.GetService<IIncomeRepository>();
        }

        /// <summary>
        /// Adds a new income record to the repository.
        /// </summary>
        /// <param name="income">The income to be added.</param>
        /// <returns>
        /// A <see cref="Task{int}"/> representing the asynchronous operation. The task result is the ID of the added income.
        /// </returns>
        public async Task<int> AddNewIncome(Income income)
        {
            try
            {
                await _incomeRepository.Add(income);
            }
            catch (Exception e)
            {
                LogError("AddNewIncome", e);
            }

            return income.Id;
        }

        /// <summary>
        /// Deletes an income record from the repository.
        /// </summary>
        /// <param name="income">The income to be deleted.</param>
        /// <returns>
        /// A <see cref="Task{int}"/> representing the asynchronous operation. The task result is the ID of the deleted income,
        /// or <c>-1</c> if the income does not exist.
        /// </returns>
        public async Task<int> DeleteIncome(Income income)
        {
            try
            {
                if (!await _incomeRepository.CheckExist(income.Id))
                    return -1;

                return await _incomeRepository.Delete(income);
            }
            catch (Exception e)
            {
                LogError("DeleteIncome", e);
                return -1;
            }
        }

        /// <summary>
        /// Searches for an income record by its source name.
        /// </summary>
        /// <param name="userId">The user ID whose income is being searched.</param>
        /// <param name="incomeSource">The source name of the income to search for.</param>
        /// <returns>
        /// A <see cref="Task{Income?}"/> representing the asynchronous operation. The task result is the income found, or <c>null</c> 
        /// if no income with the specified source name exists.
        /// </returns>
        public async Task<Income?> SearchIncomeBySourceName(int userId, string incomeSource)
        {
            try
            {
                return await _incomeRepository.GetIncomeBySourceName(userId, incomeSource);
            }
            catch (Exception e)
            {
                LogError("SearchIncomeBySourceName", e);
                return null;
            }
        }

        /// <summary>
        /// Updates an existing income record with new values.
        /// </summary>
        /// <param name="newValues">The new values for the income.</param>
        /// <returns>
        /// A <see cref="Task{Income?}"/> representing the asynchronous operation. The task result is the updated income, 
        /// or <c>null</c> if the income could not be found or updated.
        /// </returns>
        public async Task<Income?> UpdateIncome(Income newValues)
        {
            try
            {
                var oldIncome = await _incomeRepository.GetById(newValues.Id)!;
                if (oldIncome == null)
                    return null;

                oldIncome.IncomeSourceName = newValues.IncomeSourceName;
                oldIncome.Amount = newValues.Amount;
                oldIncome.IncomeDate = newValues.IncomeDate;

                await _incomeRepository.Update(oldIncome);
                return oldIncome;
            }
            catch (Exception e)
            {
                LogError("UpdateIncome", e);
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
