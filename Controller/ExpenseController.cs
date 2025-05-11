using Model.Entities;
using Model.Handlers;
using Model.Utilities;
using Controller.Validators;
using FluentValidation;

public class ExpenseController
{
    // Enum to define validation rules for various operations (Add, Update, Delete, SetReminders)
    public enum ExpenseInputValidationRules
    {
        Add,
        Update,
        Delete,
        SetReminders,
    }
    
    // Dependency injection: Initializes the ExpenseHandler and ExpenseInputValidator
    private readonly ExpenseHandler _expenseHandler = ServicesContainer.Instance.GetService<ExpenseHandler>();
    private readonly ExpenseInputValidator _validator = new ExpenseInputValidator();
    
    /// <summary>
    /// Adds a new expense after validating the input data.
    /// </summary>
    /// <param name="expense">The expense to be added.</param>
    /// <returns>A tuple indicating success, the new expense ID, and any validation errors.</returns>
    public async Task<(bool Success, int ExpenseId, List<string> Errors)> TryAddExpense(Expense expense,bool validate = false)
    {
        if (validate)
        {
            var validationResult = await _validator.ValidateAsync(expense, options => 
            {
                options.IncludeRuleSets(ExpenseInputValidationRules.Add.ToString());
            });
        
            if (!validationResult.IsValid)
            {
                return (false, -1, validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }
        }
        
        int result = await _expenseHandler.AddNewExpense(expense);
        return (result != -1, result, new List<string>());
    }

    /// <summary>
    /// Updates an existing expense after validating the new values.
    /// </summary>
    /// <param name="newValues">The new values for the expense to be updated.</param>
    /// <returns>A tuple indicating success, the updated expense, and any validation errors.</returns>
    public async Task<(bool Success, Expense? newExpense, List<string> Errors)> TryUpdateExpense(Expense newValues, bool validate = false)
    {
        if (validate)
        {
            var validationResult = await _validator.ValidateAsync(newValues, options => 
            {
                options.IncludeRuleSets(ExpenseInputValidationRules.Update.ToString());
            });
        
            if (!validationResult.IsValid)
            {
                return (false, null, validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }   
        }
        
        var updatedExpense = await _expenseHandler.UpdateExpense(newValues);
        return (updatedExpense != null, updatedExpense, new List<string>());
    }

    /// <summary>
    /// Deletes an expense after validating the expense data.
    /// </summary>
    /// <param name="expense">The expense to be deleted.</param>
    /// <returns>A tuple indicating success and any validation errors.</returns>
    public async Task<(bool Success, List<string> Errors)> TryDeleteExpense(Expense expense, bool validate = false)
    {
        if (validate)
        {
            var validationResult = await _validator.ValidateAsync(expense, options => 
            {
                options.IncludeRuleSets(ExpenseInputValidationRules.Delete.ToString());
            });
        
            if (!validationResult.IsValid)
            {
                return (false, validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }   
        }
        
        int result = await _expenseHandler.DeleteExpense(expense);
        return (result != -1, new List<string>());
    }

    /// <summary>
    /// Searches for an expense by its name and validates the input.
    /// </summary>
    /// <param name="expenseName">The name of the expense to search for.</param>
    /// <param name="userId">The ID of the user searching for the expense.</param>
    /// <returns>A tuple indicating success, the found expense (if any), and any validation errors.</returns>
    public async Task<(bool Success, Expense? expense, List<string> Errors)> TrySearchExpense(string expenseName,
        int userId, bool validate = false)
    {
        if (validate)
        {
            var validationResult = _validator.ValidateForSearch(userId, expenseName);
        
            if (!validationResult.Success)
            {
                return (false, null, validationResult.errors);
            }
        }
        
        var expense = await _expenseHandler.SearchExpenseByName(userId, expenseName);
        return (expense != null, expense, new List<string>());
    }

    /// <summary>
    /// Sets a reminder for an expense, without explicit validation (yet).
    /// </summary>
    /// <param name="expense">The expense for which the reminder will be set.</param>
    /// <param name="reminderTime">The time when the reminder should trigger.</param>
    /// <returns>A tuple indicating success and any errors (if any).</returns>
    public async Task<(bool Success, List<string> Errors)> TrySetExpenseReminder(Expense expense,
        DateTime reminderTime, bool validate = false)
    {
        bool result = await _expenseHandler.SetExpenseWithReminder(expense, reminderTime);
        return (result, new List<string>());
    }

    /// <summary>
    /// Retrieves all expenses for a user that have reminders set.
    /// </summary>
    /// <param name="userId">The ID of the user whose expenses are being retrieved.</param>
    /// <returns>A tuple indicating success, the list of expenses with reminders, and any validation errors.</returns>
    public async Task<(bool Success, List<Expense> expenses, List<string> Errors)> TryGetExpensesWithReminder(int userId,bool validate = false)
    {
        if (validate)
        {
            var intValidator = new InlineValidator<int>();
            intValidator.RuleFor(i => i).GreaterThan(0).WithMessage("UserId can't be 0 or negative");
        
            var validationResult = await intValidator.ValidateAsync(userId);
            if (!validationResult.IsValid)
            {
                return (false, new List<Expense>(), validationResult.Errors.Select(e => e.ErrorMessage).ToList());
            }   
        }
        
        var expenses = await _expenseHandler.GetExpensesThatHasReminders(userId);
        return (expenses.Count != 0, expenses, new List<string>());
    }
}
