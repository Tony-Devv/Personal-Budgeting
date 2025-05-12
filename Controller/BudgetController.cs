using Controller.Validators;
using FluentValidation;
using FluentValidation.Internal;
using Model.Entities;
using Model.Handlers;
using Model.Utilities;

public class BudgetController
{
    /// <summary>
    /// Enumeration representing the different validation rules for budget input operations.
    /// </summary>
    public enum BudgetInputValidationRules
    {
        AddNew,
        Update,
        Delete,
        SearchByName
    }

    private readonly BudgetHandler _budgetHandler = 
        ServicesContainer.Instance.GetService<BudgetHandler>();

    private readonly BudgetInputValidator _budgetInputValidator = new BudgetInputValidator();

    /// <summary>
    /// Attempts to get a budget by its name for a specific user.
    /// </summary>
    /// <param name="userId">The ID of the user requesting the budget.</param>
    /// <param name="budgetName">The name of the budget to retrieve.</param>
    /// <returns>A tuple containing a success flag and a list of errors (if any).</returns>
    public async Task<(bool Success, List<string> errors)> TryGetBudgetByName(int userId, string budgetName,bool validate = false)
    {
        if (validate)
        {
            var validationResult = _budgetInputValidator.ValidateForSearch(userId, budgetName);

            if (!validationResult.Success)
            {
                return (validationResult.Success, validationResult.errors);
            }    
        }
        
        var budget = await _budgetHandler.GetBudgetByName(userId, budgetName);
        return (budget != null, new List<string>());
    }

    /// <summary>
    /// Attempts to add a new budget after validating the input.
    /// </summary>
    /// <param name="budget">The budget object to add.</param>
    /// <returns>A tuple containing a success flag, the ID of the added budget, and any errors (if any).</returns>
    public async Task<(bool Success, int BudgetId, List<string> errors)> TryAddBudget(Budget budget,bool validate = false)
    {
        if (validate)
        {
            var context = CreateContext(budget, BudgetInputValidationRules.AddNew);

            var validationResult = await _budgetInputValidator.ValidateAsync(context);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return (false, -1, errors);
            }    
        }

        int result = await _budgetHandler.AddNewBudget(budget);
        return (result != -1, result, new List<string>());
    }

    /// <summary>
    /// Attempts to update an existing budget after validating the input.
    /// </summary>
    /// <param name="newValues">The new values for the budget.</param>
    /// <returns>A tuple containing a success flag, the updated budget, and any errors (if any).</returns>
    public async Task<(bool Success, Budget? BudgetId, List<string> errors)> TryUpdateBudget(Budget newValues,bool validate = false)
    {
        if (validate)
        {
            var context = CreateContext(newValues, BudgetInputValidationRules.Update);

            var validationResult = await _budgetInputValidator.ValidateAsync(context);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return (false, null, errors);
            }   
        }

        var updatedBudget = await _budgetHandler.UpdateBudget(newValues);
        return (updatedBudget != null, updatedBudget, new List<string>());
    }

    /// <summary>
    /// Attempts to delete a budget after validating the input.
    /// </summary>
    /// <param name="budget">The budget to delete.</param>
    /// <returns>A tuple containing a success flag and any errors (if any).</returns>
    public async Task<(bool Success, List<string> errors)> TryDeleteBudget(Budget budget,bool validate = false)
    {
        if (validate)
        {
            var context = CreateContext(budget, BudgetInputValidationRules.Delete);

            var validationResult = await _budgetInputValidator.ValidateAsync(context);

            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
                return (false, errors);
            }   
        }

        int result = await _budgetHandler.DeleteBudget(budget);
        return (result != -1, new List<string>());
    }

    /// <summary>
    /// Creates a validation context for a specific budget input and validation rules.
    /// </summary>
    /// <param name="budgetInput">The budget input to validate.</param>
    /// <param name="validationsRules">The validation rules to apply.</param>
    /// <returns>A validation context for the budget input.</returns>
    private ValidationContext<Budget> CreateContext(Budget budgetInput, BudgetInputValidationRules validationsRules)
    {
        var context = new ValidationContext<Budget>(budgetInput, new PropertyChain(),
            new RulesetValidatorSelector(new[] { validationsRules.ToString() }));

        return context;
    }
}
