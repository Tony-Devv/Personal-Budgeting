using Controller.Validators;
using FluentValidation;
using FluentValidation.Internal;
using Model.Entities;
using Model.Handlers;
using Model.Utilities;

public class BudgetController
{
    
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

    
    public async Task<(bool Success,List<string> errors)> TryGetBudgetByName(int userId, string budgetName)
    {
        /*
        var validationResult = _budgetInputValidator.ValidateForSearch(userId, budgetName);

        if (!validationResult.Success)
        {
            return (validationResult.Success, validationResult.errors);
        }
        */
        
        var budget = await _budgetHandler.GetBudgetByName(userId, budgetName);
        return (budget != null,new List<string>());
    }

    public async Task<(bool Success, int BudgetId,List<string> errors)> TryAddBudget(Budget budget)
    {
        /*
        var context = CreateContext(budget, BudgetInputValidationRules.AddNew);

        var validationResult = await _budgetInputValidator.ValidateAsync(context);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return (false, -1, errors);
        }
        */
        
        int result = await _budgetHandler.AddNewBudget(budget);
        return (result != -1, result,new List<string>());
    }

    public async Task<(bool Success, Budget? BudgetId,List<string> errors)> TryUpdateBudget(Budget newValues)
    {/*
        var context = CreateContext(newValues, BudgetInputValidationRules.Update);
        
        var validationResult = await _budgetInputValidator.ValidateAsync(context);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return (false,null, errors);
        }
        */
        
        var updatedBudget = await _budgetHandler.UpdateBudget(newValues);
        return (updatedBudget != null, updatedBudget, new List<string>());
    }

    public async Task<(bool Success,List<string> errors)> TryDeleteBudget(Budget budget)
    {
        /*
        var context = CreateContext(budget, BudgetInputValidationRules.Delete);
        
        var validationResult = await _budgetInputValidator.ValidateAsync(context);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return (false, errors);
        }
        */
        
        int result = await _budgetHandler.DeleteBudget(budget);
        return (result != -1,new List<string>());
    }

    private ValidationContext<Budget> CreateContext(Budget budgetInput,BudgetInputValidationRules validationsRules)
    {
        var context = new ValidationContext<Budget>(budgetInput,new PropertyChain(),
            new RulesetValidatorSelector(new[] { validationsRules.ToString() }));

        return context;
    } 
}
