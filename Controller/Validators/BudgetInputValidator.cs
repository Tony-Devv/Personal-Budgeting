using FluentValidation;
using Model.Entities;

namespace Controller.Validators;

public class BudgetInputValidator : AbstractValidator<Budget>
{

    public BudgetInputValidator()
    {
        RuleSet(BudgetController.BudgetInputValidationRules.AddNew.ToString(), () =>
        {
            RuleFor(b => b.UserId).NotEmpty().WithMessage("Their Must be a UserId before creating a budget")
                .GreaterThan(0).WithMessage("User Id Can't Be negative");

            RuleFor(b => b.Id).Empty().WithMessage("There can't be an id when adding");

            RuleFor(b => b.BudgetName).NotEmpty().WithMessage("Budget Name can't be empty")
                .MaximumLength(100).WithMessage("Maximum Allowed Length is 100")
                .MinimumLength(3).WithMessage("Minimum Length is 3");

            RuleFor(b => b.TotalAmountRequired).GreaterThan(0).WithMessage("Can't be less than or equal to zero")
                .Must(d => d.ToString().Length <= 10).WithMessage("Can't be more than 10 digits");
        });
        
        RuleSet(BudgetController.BudgetInputValidationRules.Update.ToString(), () =>
        {
            RuleFor(b => b.Id).GreaterThan(0).WithMessage("Id Can't be negative or equal to zero");

            RuleFor(b => b.UserId).GreaterThan(0).WithMessage("UserId can't be negative or equal to zero");
            
            RuleFor(b => b.BudgetName).NotEmpty().WithMessage("Budget Name can't be empty")
                .MaximumLength(100).WithMessage("Maximum Allowed Length is 100")
                .MinimumLength(3).WithMessage("Minimum Length is 3");

            RuleFor(b => b.TotalAmountRequired).GreaterThan(0).WithMessage("Can't be less than or equal to zero")
                .Must(d => d.ToString().Length <= 10).WithMessage("Can't be more than 10 digits");
        });
        
        RuleSet(BudgetController.BudgetInputValidationRules.Delete.ToString(), () =>
        {
            RuleFor(b => b.Id).GreaterThan(0).WithMessage("The id can't be 0 or negative");
        });
    }
    
    public (bool Success,List<string> errors) ValidateForSearch(int userId,string budgetName)
    {
        var errors = new List<string>();
        var stringValidator = new InlineValidator<string>();
        var intValidator = new InlineValidator<int>();

        stringValidator.RuleFor(b => b).NotEmpty().WithMessage("Budget Name can't be empty")
            .MaximumLength(100).WithMessage("Maximum Allowed Length is 100")
            .MinimumLength(3).WithMessage("Minimum Length is 3");

        intValidator.RuleFor(i => i).GreaterThan(0).WithMessage("UserId can't be 0 or negative");

        var validationResult_budget = stringValidator.Validate(budgetName);
        var validationResult_userId = intValidator.Validate(userId);
        
        bool validationResult = validationResult_budget.IsValid && validationResult_userId.IsValid;

        if (!validationResult)
        {
            errors = validationResult_budget.Errors.Select(e => e.ErrorMessage).ToList();
            errors.AddRange(validationResult_userId.Errors.Select(e => e.ErrorMessage).ToList());
        }  
        
        return (validationResult,errors);
    }
}