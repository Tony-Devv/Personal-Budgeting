using Controller.Validators;
using FluentValidation;
using FluentValidation.Internal;
using Model.Entities;
using Model.Handlers;
using Model.Utilities;

public class IncomeController
{
    public enum IncomeInputValidationRules
    {
        AddNew,
        Update,
        Delete,
        SearchByName
    }

    private readonly IncomeHandler _incomeHandler =
        ServicesContainer.Instance.GetService<IncomeHandler>();

    private readonly IncomeValidator _incomeValidator = new IncomeValidator();

    public async Task<(bool Success, int IncomeId, List<string> errors)> TryAddIncome(Income income)
    {/*
        var context = CreateContext(income, IncomeInputValidationRules.AddNew);
        var validationResult = await _incomeValidator.ValidateAsync(context);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return (false, -1, errors);
        }
        */

        int result = await _incomeHandler.AddNewIncome(income);
        return (result != -1, result, new List<string>());
    }

    public async Task<(bool Success, Income? updatedIncome, List<string> errors)> TryUpdateIncome(Income newValues)
    {/*
        var context = CreateContext(newValues, IncomeInputValidationRules.Update);
        var validationResult = await _incomeValidator.ValidateAsync(context);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return (false, null, errors);
        }
        */

        var updatedIncome = await _incomeHandler.UpdateIncome(newValues);
        return (updatedIncome != null, updatedIncome, new List<string>());
    }

    public async Task<(bool Success, List<string> errors)> TryDeleteIncome(Income income)
    {/*
        var context = CreateContext(income, IncomeInputValidationRules.Delete);
        var validationResult = await _incomeValidator.ValidateAsync(context);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return (false, errors);
        }
        */

        int result = await _incomeHandler.DeleteIncome(income);
        return (result != -1, new List<string>());
    }

    public async Task<(bool Success, List<string> errors)> TrySearchIncome(int userId, string incomeSourceName)
    {/*
        var validationResult = _incomeValidator.ValidateForSearch(userId, incomeSourceName);

        if (!validationResult.Success)
        {
            return (false, validationResult.Errors);
        }
        */

        var income = await _incomeHandler.SearchIncomeBySourceName(userId, incomeSourceName);
        return (income != null, new List<string>());
    }

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
