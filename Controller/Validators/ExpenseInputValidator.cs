using FluentValidation;
using Model.Entities;

namespace Controller.Validators;

public class ExpenseInputValidator : AbstractValidator<Expense>
{
    public ExpenseInputValidator()
    {
        RuleSet(
             ExpenseController.ExpenseInputValidationRules.Add.ToString(),
             () =>
             {
                 RuleFor(e => e.Id).Empty().WithMessage("Id Can't Contain a value when adding");
                 RuleFor(e => e.UserId).GreaterThan(0).WithMessage("UserId must be greater than 0 and exist");
                 RuleFor(e => e.BudgetId).GreaterThan(0).WithMessage("BudgetId must be greater than 0 and exist");

                 RuleFor(e => e.ExpenseName).NotEmpty().WithMessage("Expense Name Can't be empty")
                     .MinimumLength(3).WithMessage("Minimum Length is 3")
                     .MaximumLength(100).WithMessage("Maximum Length is 100");

                 RuleFor(e => e.RequiredAmount).GreaterThan(0).WithMessage("Required Amount Must be greater than 0")
                     .Must(e => e.ToString().Length < 6).WithMessage("Can't be more than 6 digits");

                 RuleFor(e => e.SpentAmount).GreaterThan(0).WithMessage("SpentAmount  Must be greater than 0")
                     .Must(e => e.ToString().Length < 6).WithMessage("Can't be more than 6 digits");

                 
                 RuleFor(e => e.DateCycle).NotEmpty().WithMessage("DateCycle Can't Be Empty");

                 RuleFor(e => e.ReminderTime).Empty()
                     .WithMessage("Reminder Time must not be initialized while adding new Expenses");
             });
        
        RuleSet(ExpenseController.ExpenseInputValidationRules.Update.ToString(), () =>
        {
            RuleFor(e => e.Id).GreaterThan(0).WithMessage("Id Must be Greater than 0 and exist when updating");
            RuleFor(e => e.UserId).GreaterThan(0).WithMessage("UserId must be greater than 0 and exist");
            RuleFor(e => e.BudgetId).GreaterThan(0).WithMessage("BudgetId must be greater than 0 and exist");

            RuleFor(e => e.ExpenseName).NotEmpty().WithMessage("Expense Name Can't be empty")
                .MinimumLength(3).WithMessage("Minimum Length is 3")
                .MaximumLength(100).WithMessage("Maximum Length is 100");

            RuleFor(e => e.RequiredAmount).GreaterThan(0).WithMessage("Required Amount Must be greater than 0")
                .Must(e => e.ToString().Length < 6).WithMessage("Can't be more than 6 digits");

            RuleFor(e => e.SpentAmount).GreaterThan(0).WithMessage("SpentAmount  Must be greater than 0")
                .Must(e => e.ToString().Length < 6).WithMessage("Can't be more than 6 digits");

                 
            RuleFor(e => e.DateCycle).NotEmpty().WithMessage("DateCycle Can't Be Empty");

            RuleFor(e => e.ReminderTime).Empty()
                .WithMessage("Reminder Time must not be initialized while adding new Expenses");
        });
        
        RuleSet(ExpenseController.ExpenseInputValidationRules.Delete.ToString(), () =>
        {
            RuleFor(e => e.Id).GreaterThan(0).WithMessage("The id can't be 0 or negative");
        });
        
        
        RuleSet(ExpenseController.ExpenseInputValidationRules.SetReminders.ToString(), () =>
        {
            RuleFor(e => e.Id).GreaterThan(0).WithMessage("Id Must be Greater than 0 and exist");
            RuleFor(e => e.UserId).GreaterThan(0).WithMessage("UserId must be greater than 0 and exist");
            RuleFor(e => e.BudgetId).GreaterThan(0).WithMessage("BudgetId must be greater than 0 and exist");

            RuleFor(e => e.ExpenseName).NotEmpty().WithMessage("Expense Name Can't be empty")
                .MinimumLength(3).WithMessage("Minimum Length is 3")
                .MaximumLength(100).WithMessage("Maximum Length is 100");

            RuleFor(e => e.RequiredAmount).GreaterThan(0).WithMessage("Required Amount Must be greater than 0")
                .Must(e => e.ToString().Length < 6).WithMessage("Can't be more than 6 digits");

            RuleFor(e => e.SpentAmount).GreaterThan(0).WithMessage("SpentAmount  Must be greater than 0")
                .Must(e => e.ToString().Length < 6).WithMessage("Can't be more than 6 digits");
                 
            RuleFor(e => e.DateCycle).NotEmpty().WithMessage("DateCycle Can't Be Empty");

            RuleFor(e => e.ReminderTime).NotEmpty()
                .WithMessage("ReminderTime Must not be Empty if your adding reminders");
        });
        
    }
    
    public (bool Success,List<string> errors) ValidateForSearch(int userId,string budgetName)
    {
        var errors = new List<string>();
        var stringValidator = new InlineValidator<string>();
        var intValidator = new InlineValidator<int>();

        stringValidator.RuleFor(b => b).NotEmpty().WithMessage("ExpenseName Name can't be empty")
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