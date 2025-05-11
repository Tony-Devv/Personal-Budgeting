using FluentValidation;
using Model.Entities;

namespace Controller.Validators
{
    /// <summary>
    /// Validator class for validating inputs related to expense management.
    /// This class uses FluentValidation to define validation rules for various expense operations such as add, update, delete, and setting reminders.
    /// </summary>
    public class ExpenseInputValidator : AbstractValidator<Expense>
    {
        public ExpenseInputValidator()
        {
            // Validation rules for adding a new expense
            RuleSet(ExpenseController.ExpenseInputValidationRules.Add.ToString(), () =>
            {
                RuleFor(e => e.Id).Empty().WithMessage("Id Can't Contain a value when adding");
                RuleFor(e => e.UserId).GreaterThan(0).WithMessage("UserId must be greater than 0 and exist");
                RuleFor(e => e.BudgetId).GreaterThan(0).WithMessage("BudgetId must be greater than 0 and exist");

                RuleFor(e => e.ExpenseName).NotEmpty().WithMessage("Expense Name Can't be empty")
                    .MinimumLength(3).WithMessage("Minimum Length is 3")
                    .MaximumLength(100).WithMessage("Maximum Length is 100");

                RuleFor(e => e.RequiredAmount).GreaterThan(0).WithMessage("Required Amount Must be greater than 0")
                    .Must(e => e.ToString().Length < 6).WithMessage("Can't be more than 6 digits");

                RuleFor(e => e.SpentAmount).GreaterThan(0).WithMessage("SpentAmount Must be greater than 0")
                    .Must(e => e.ToString().Length < 6).WithMessage("Can't be more than 6 digits");

                RuleFor(e => e.DateCycle).NotEmpty().WithMessage("DateCycle Can't Be Empty");

                RuleFor(e => e.ReminderTime).Empty()
                    .WithMessage("Reminder Time must not be initialized while adding new Expenses");
            });

            // Validation rules for updating an existing expense
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

                RuleFor(e => e.SpentAmount).GreaterThan(0).WithMessage("SpentAmount Must be greater than 0")
                    .Must(e => e.ToString().Length < 6).WithMessage("Can't be more than 6 digits");

                RuleFor(e => e.DateCycle).NotEmpty().WithMessage("DateCycle Can't Be Empty");

                RuleFor(e => e.ReminderTime).Empty()
                    .WithMessage("Reminder Time must not be initialized while adding new Expenses");
            });

            // Validation rules for deleting an expense
            RuleSet(ExpenseController.ExpenseInputValidationRules.Delete.ToString(), () =>
            {
                RuleFor(e => e.Id).GreaterThan(0).WithMessage("The id can't be 0 or negative");
            });

            // Validation rules for setting reminders on an expense
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

                RuleFor(e => e.SpentAmount).GreaterThan(0).WithMessage("SpentAmount Must be greater than 0")
                    .Must(e => e.ToString().Length < 6).WithMessage("Can't be more than 6 digits");

                RuleFor(e => e.DateCycle).NotEmpty().WithMessage("DateCycle Can't Be Empty");

                RuleFor(e => e.ReminderTime).NotEmpty()
                    .WithMessage("ReminderTime Must not be Empty if you're adding reminders");
            });
        }

        /// <summary>
        /// Validates the inputs for searching an expense based on user ID and budget name.
        /// </summary>
        /// <param name="userId">The user ID to validate.</param>
        /// <param name="budgetName">The budget name to validate.</param>
        /// <returns>A tuple containing a success flag and a list of error messages if validation fails.</returns>
        public (bool Success, List<string> errors) ValidateForSearch(int userId, string budgetName)
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

            return (validationResult, errors);
        }
    }
}
