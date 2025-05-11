using FluentValidation;
using Model.Entities;

namespace Controller.Validators
{
    /// <summary>
    /// Validator class for validating `Budget` objects based on different rules for adding, updating, and deleting.
    /// </summary>
    public class BudgetInputValidator : AbstractValidator<Budget>
    {
        public BudgetInputValidator()
        {
            // Validation rules for adding a new budget
            RuleSet(BudgetController.BudgetInputValidationRules.AddNew.ToString(), () =>
            {
                RuleFor(b => b.UserId).NotEmpty().WithMessage("There Must be a UserId before creating a budget")
                    .GreaterThan(0).WithMessage("User Id Can't Be negative");

                RuleFor(b => b.Id).Empty().WithMessage("There can't be an id when adding");

                RuleFor(b => b.BudgetName).NotEmpty().WithMessage("Budget Name can't be empty")
                    .MaximumLength(100).WithMessage("Maximum Allowed Length is 100")
                    .MinimumLength(3).WithMessage("Minimum Length is 3");

                RuleFor(b => b.TotalAmountRequired).GreaterThan(0).WithMessage("Can't be less than or equal to zero")
                    .Must(d => d.ToString().Length <= 10).WithMessage("Can't be more than 10 digits");
            });

            // Validation rules for updating an existing budget
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

            // Validation rules for deleting a budget
            RuleSet(BudgetController.BudgetInputValidationRules.Delete.ToString(), () =>
            {
                RuleFor(b => b.Id).GreaterThan(0).WithMessage("The id can't be 0 or negative");
            });
        }

        /// <summary>
        /// Validates the input for searching a budget by user ID and budget name.
        /// </summary>
        /// <param name="userId">The ID of the user searching for the budget.</param>
        /// <param name="budgetName">The name of the budget being searched for.</param>
        /// <returns>A tuple indicating whether the validation was successful and any error messages.</returns>
        public (bool Success, List<string> errors) ValidateForSearch(int userId, string budgetName)
        {
            var errors = new List<string>();
            var stringValidator = new InlineValidator<string>();
            var intValidator = new InlineValidator<int>();

            // String validation for the budget name
            stringValidator.RuleFor(b => b).NotEmpty().WithMessage("Budget Name can't be empty")
                .MaximumLength(100).WithMessage("Maximum Allowed Length is 100")
                .MinimumLength(3).WithMessage("Minimum Length is 3");

            // Integer validation for the user ID
            intValidator.RuleFor(i => i).GreaterThan(0).WithMessage("UserId can't be 0 or negative");

            // Validate the budget name and user ID
            var validationResult_budget = stringValidator.Validate(budgetName);
            var validationResult_userId = intValidator.Validate(userId);

            bool validationResult = validationResult_budget.IsValid && validationResult_userId.IsValid;

            // If validation fails, collect the error messages
            if (!validationResult)
            {
                errors = validationResult_budget.Errors.Select(e => e.ErrorMessage).ToList();
                errors.AddRange(validationResult_userId.Errors.Select(e => e.ErrorMessage).ToList());
            }

            return (validationResult, errors);
        }
    }
}
