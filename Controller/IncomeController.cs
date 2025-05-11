using Controller.Validators;
using FluentValidation;
using FluentValidation.Internal;
using Model.Entities;
using Model.Handlers;
using Model.Utilities;

public class IncomeController
{
    /// <summary>
    /// Enum representing different income input validation rules.
    /// </summary>
    public enum IncomeInputValidationRules
    {
        AddNew,        // Rule set for adding new income
        Update,        // Rule set for updating existing income
        Delete,        // Rule set for deleting income
        SearchByName   // Rule set for searching income by name
    }

    private readonly IncomeHandler _incomeHandler =
        ServicesContainer.Instance.GetService<IncomeHandler>();

    private readonly IncomeValidator _incomeValidator = new IncomeValidator();

    /// <summary>
    /// Attempts to add a new income record, validating input and returning errors if necessary.
    /// </summary>
    /// <param name="income">The income object to be added.</param>
    /// <returns>A tuple containing success flag, the added income ID, and a list of error messages if any.</returns>
    public async Task<(bool Success, int IncomeId, List<string> errors)> TryAddIncome(Income income)
    {
        var context = CreateContext(income, IncomeInputValidationRules.AddNew);
        var validationResult = await _incomeValidator.ValidateAsync(context);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return (false, -1, errors);
        }

        int result = await _incomeHandler.AddNewIncome(income);
        return (result != -1, result, new List<string>());
    }

    /// <summary>
    /// Attempts to update an existing income record, validating input and returning errors if necessary.
    /// </summary>
    /// <param name="newValues">The new values to update the income with.</param>
    /// <returns>A tuple containing success flag, the updated income object, and a list of error messages if any.</returns>
    public async Task<(bool Success, Income? updatedIncome, List<string> errors)> TryUpdateIncome(Income newValues)
    {
        var context = CreateContext(newValues, IncomeInputValidationRules.Update);
        var validationResult = await _incomeValidator.ValidateAsync(context);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return (false, null, errors);
        }

        var updatedIncome = await _incomeHandler.UpdateIncome(newValues);
        return (updatedIncome != null, updatedIncome, new List<string>());
    }

    /// <summary>
    /// Attempts to delete an income record, validating input and returning errors if necessary.
    /// </summary>
    /// <param name="income">The income object to be deleted.</param>
    /// <returns>A tuple containing success flag and a list of error messages if any.</returns>
    public async Task<(bool Success, List<string> errors)> TryDeleteIncome(Income income)
    {
        var context = CreateContext(income, IncomeInputValidationRules.Delete);
        var validationResult = await _incomeValidator.ValidateAsync(context);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return (false, errors);
        }

        int result = await _incomeHandler.DeleteIncome(income);
        return (result != -1, new List<string>());
    }

    /// <summary>
    /// Attempts to search for income records by user ID and income source name, validating input and returning errors if necessary.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="incomeSourceName">The income source name to search for.</param>
    /// <returns>A tuple containing success flag and a list of error messages if any.</returns>
    public async Task<(bool Success, List<string> errors)> TrySearchIncome(int userId, string incomeSourceName)
    {
        var validationResult = _incomeValidator.ValidateForSearch(userId, incomeSourceName);

        if (!validationResult.Success)
        {
            return (false, validationResult.Errors);
        }

        var income = await _incomeHandler.SearchIncomeBySourceName(userId, incomeSourceName);
        return (income != null, new List<string>());
    }

    /// <summary>
    /// Creates a validation context for the income object with the specified validation rule set.
    /// </summary>
    /// <param name="incomeInput">The income object to validate.</param>
    /// <param name="validationRule">The validation rule set to apply.</param>
    /// <returns>A validation context initialized with the income object and validation rule.</returns>
    private ValidationContext<Income> CreateContext(Income incomeInput, IncomeInputValidationRules validationRule)
    {
        var context = new ValidationContext<Income>(
            incomeInput,
            new PropertyChain(),
            new RulesetValidatorSelector(new[] { validationRule.ToString() })
        );

        return context;
    }
}
