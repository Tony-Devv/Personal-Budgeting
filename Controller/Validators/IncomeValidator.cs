using FluentValidation;
using Model.Entities;

namespace Controller.Validators
{
    public class IncomeValidator : AbstractValidator<Income>
    {

        public IncomeValidator()
        {
            RuleSet(IncomeController.IncomeInputValidationRules.AddNew.ToString(), () =>
            {
                RuleFor(i => i.Id)
                    .Empty().WithMessage("Id must be empty when adding a new income");

                RuleFor(i => i.UserId)
                    .GreaterThan(0).WithMessage("UserId must be greater than 0");

                RuleFor(i => i.Amount)
                    .GreaterThan(0).WithMessage("Amount must be greater than 0")
                    .Must(d => d.ToString().Length <= 10).WithMessage("Amount can't be more than 10 digits");

                RuleFor(i => i.IncomeSourceName)
                    .NotEmpty().WithMessage("Income source name is required")
                    .MinimumLength(3).WithMessage("Minimum length for income source name is 3")
                    .MaximumLength(100).WithMessage("Maximum length for income source name is 100");

                RuleFor(i => i.IncomeDate)
                    .NotEmpty().WithMessage("Income date is required");
            });

            RuleSet(IncomeController.IncomeInputValidationRules.Update.ToString(), () =>
            {
                RuleFor(i => i.Id)
                    .GreaterThan(0).WithMessage("Id must be greater than 0");

                RuleFor(i => i.UserId)
                    .GreaterThan(0).WithMessage("UserId must be greater than 0");

                RuleFor(i => i.Amount)
                    .GreaterThan(0).WithMessage("Amount must be greater than 0")
                    .Must(d => d.ToString().Length <= 10).WithMessage("Amount can't be more than 10 digits");

                RuleFor(i => i.IncomeSourceName)
                    .NotEmpty().WithMessage("Income source name is required")
                    .MinimumLength(3).WithMessage("Minimum length for income source name is 3")
                    .MaximumLength(100).WithMessage("Maximum length for income source name is 100");

                RuleFor(i => i.IncomeDate)
                    .NotEmpty().WithMessage("Income date is required");
            });

            RuleSet(IncomeController.IncomeInputValidationRules.Delete.ToString(), () =>
            {
                RuleFor(i => i.Id)
                    .GreaterThan(0).WithMessage("Id must be greater than 0");
            });
        }

        public (bool Success, List<string> Errors) ValidateForSearch(int userId, string incomeSourceName)
        {
            var errors = new List<string>();

            var stringValidator = new InlineValidator<string>();
            stringValidator.RuleFor(name => name)
                .NotEmpty().WithMessage("Income source name is required")
                .MinimumLength(3).WithMessage("Minimum length for income source name is 3")
                .MaximumLength(100).WithMessage("Maximum length for income source name is 100");

            var intValidator = new InlineValidator<int>();
            intValidator.RuleFor(id => id)
                .GreaterThan(0).WithMessage("UserId must be greater than 0");

            var nameValidation = stringValidator.Validate(incomeSourceName);
            var idValidation = intValidator.Validate(userId);

            bool isValid = nameValidation.IsValid && idValidation.IsValid;

            if (!isValid)
            {
                errors.AddRange(nameValidation.Errors.Select(e => e.ErrorMessage));
                errors.AddRange(idValidation.Errors.Select(e => e.ErrorMessage));
            }

            return (isValid, errors);
        }
    }
}
