using FluentValidation;
using Model.Entities;

namespace Controller.Validators
{
    /// <summary>
    /// Validator class for validating user input data for various user-related operations such as registration, login, and profile editing.
    /// Uses FluentValidation to define validation rules for different user actions.
    /// </summary>
    public class UserInputValidator : AbstractValidator<User>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UserInputValidator"/> class.
        /// Defines validation rules for various user actions such as registration, login, profile editing, password change, and data retrieval.
        /// </summary>
        public UserInputValidator()
        {
            // Validation rules for user registration
            RuleSet(UserController.UserValidationsRules.Register.ToString(), () =>
            {
                RuleFor(u => u.Id).Empty().WithMessage("Id should be empty during registration.");

                RuleFor(u => u.UserName).NotEmpty().WithMessage("UserName can't be empty")
                    .MinimumLength(4).WithMessage("UserName must be at least 4 characters")
                    .MaximumLength(100).WithMessage("UserName cannot exceed 100 characters");

                RuleFor(u => u.Email).NotEmpty().WithMessage("Email can't be empty")
                    .Matches("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$").WithMessage("Invalid email format")
                    .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

                RuleFor(u => u.PhoneNumber).NotEmpty().WithMessage("Phone number can't be empty")
                    .Matches("^01[0125]\\d{8}$").WithMessage("Invalid phone number format (Egypt only)");

                RuleFor(u => u.Password).NotEmpty().WithMessage("Password can't be empty")
                    .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
                    .MaximumLength(255).WithMessage("Password cannot exceed 255 characters");
            });

            // Validation rules for user login
            RuleSet(UserController.UserValidationsRules.Login.ToString(), () =>
            {
                RuleFor(u => u.Id).Empty().WithMessage("Don't include Id during login");

                RuleFor(u => u.Email).NotEmpty().WithMessage("Email can't be empty")
                    .Matches("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$").WithMessage("Invalid email format")
                    .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

                // Password validation omitted for login (can be handled separately in another rule set)
            });

            // Validation rules for editing user details
            RuleSet(UserController.UserValidationsRules.Edit.ToString(), () =>
            {
                RuleFor(u => u.Id).NotEmpty().WithMessage("Id can't be empty when editing details");

                RuleFor(u => u.UserName).NotEmpty().WithMessage("UserName can't be empty")
                    .MinimumLength(4).WithMessage("UserName must be at least 4 characters")
                    .MaximumLength(100).WithMessage("UserName cannot exceed 100 characters");

                RuleFor(u => u.Email).NotEmpty().WithMessage("Email can't be empty")
                    .EmailAddress().WithMessage("Invalid email format")
                    .MaximumLength(100).WithMessage("Email cannot exceed 100 characters");

                RuleFor(u => u.PhoneNumber).NotEmpty().WithMessage("Phone number can't be empty")
                    .Matches("^01[0125]\\d{8}$").WithMessage("Invalid phone number format (Egypt only)");

                RuleFor(u => u.Password).Empty().WithMessage("Do not include password if you're editing other details");
            });

            // Validation rules for changing password
            RuleSet(UserController.UserValidationsRules.ChangePassword.ToString(), () =>
            {
                RuleFor(u => u.Id).NotEmpty().WithMessage("Id can't be empty when changing password");

                RuleFor(u => u.Password).NotEmpty().WithMessage("Old password must be entered")
                    .MinimumLength(6).WithMessage("Password must be at least 6 characters long");
            });

            // Validation rules for retrieving user data
            RuleSet(UserController.UserValidationsRules.GetUserData.ToString(), () =>
            {
                RuleFor(u => u.Id).NotEmpty().WithMessage("Id can't be empty when accessing user data");
            });
        }

        /// <summary>
        /// Validates a single string (e.g., a password) for certain conditions.
        /// </summary>
        /// <param name="input">The string input to validate.</param>
        /// <returns>
        /// A tuple containing a boolean indicating whether the validation was successful, 
        /// and a list of error messages if validation failed.
        /// </returns>
        public (bool Success, List<string> Errors) ValidateSingleString_ForPassword(string input)
        {
            var errors = new List<string>();
            var stringValidator = new InlineValidator<string>();

            // Password validation rules
            stringValidator.RuleFor(x => x).NotEmpty().WithMessage("New password can't be empty");
            stringValidator.RuleFor(x => x).MinimumLength(4).WithMessage("Password length must be at least 4 characters");

            var validationResult = stringValidator.Validate(input);
            errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();

            return (validationResult.IsValid, errors);
        }
    }
}
