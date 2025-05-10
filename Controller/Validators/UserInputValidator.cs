using FluentValidation;
using Model.Entities;

namespace Controller.Validators;

public class UserInputValidator : AbstractValidator<User>
{
    public UserInputValidator()
    {
        RuleSet(UserController.UserValidationsRules.Register.ToString(), () =>
        {
            RuleFor(u => u.Id).Empty();

            RuleFor(u => u.UserName).NotEmpty().WithMessage("UserName can't Be Empty")
                .MinimumLength(4).WithMessage("UserName have to be at least 4")
                .MaximumLength(100).WithMessage("UserName Maximum can be 100 character");

            RuleFor(u => u.Email).NotEmpty().WithMessage("Email Can't Be Empty")
                .Matches("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$").WithMessage("This is not a valid Email")
                .MaximumLength(100).WithMessage("Maximum Length is 100");

            RuleFor(u => u.PhoneNumber).NotEmpty().WithMessage("Phone Number can't be Empty")
                .Matches("^01[0125]\\d{8}$").WithMessage("Not a valid Egypt PhoneNumber, try again");

            RuleFor(u => u.Password).NotEmpty().WithMessage("Password Can't Be Empty")
                .MinimumLength(6).WithMessage("Minimum Password Length is 6")
                .MaximumLength(255).WithMessage("Maximum Password Length is 255");
        });
        
        
        
        
        RuleSet(UserController.UserValidationsRules.Login.ToString(), () =>
        {
            RuleFor(u => u.Id).Empty().WithMessage("Don't Include Id in login");
            
            RuleFor(u => u.Email).NotEmpty().WithMessage("Email Can't Be Empty")
                .Matches("^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\\.[a-zA-Z]{2,}$").WithMessage("This is not a valid Email")
                .MaximumLength(100).WithMessage("Maximum Length is 100");
            
            /*
            RuleFor(u => u.Password).NotEmpty().WithMessage("Password Can't Be Empty")
                .MinimumLength(6).WithMessage("Minimum Password Length is 6")
                .MaximumLength(255).WithMessage("Maximum Password Length is 255");
        */
        });
        
        
        RuleSet(UserController.UserValidationsRules.Edit.ToString(), () =>
        {
            RuleFor(u => u.Id).NotEmpty().WithMessage("Id Can't Be Empty if your trying to Edit");
            
            RuleFor(u => u.UserName).NotEmpty().WithMessage("UserName can't Be Empty")
                .MinimumLength(4).WithMessage("UserName have to be at least 4")
                .MaximumLength(100).WithMessage("UserName Maximum can be 100 character");

            RuleFor(u => u.Email).NotEmpty().WithMessage("Email Can't Be Empty")
                .EmailAddress().WithMessage("This is not a valid Email")
                .MaximumLength(100).WithMessage("Maximum Length is 100");

            RuleFor(u => u.PhoneNumber).NotEmpty().WithMessage("Phone Number can't be Empty")
                .Matches("^01[0125]\\d{8}$").WithMessage("Not a valid Egypt PhoneNumber, try again");

            RuleFor(u => u.Password).Empty()
                .WithMessage("Don't Include Password in the input if your editing details only");
        });


        RuleSet(UserController.UserValidationsRules.ChangePassword.ToString(),
            () =>
            {
                RuleFor(u => u.Id).NotEmpty().WithMessage("Id Can't Be Empty When Changing Password");

                RuleFor(u => u.Password).NotEmpty().WithMessage("Old Password Must be Entered").MinimumLength(6)
                    .WithMessage("Minimum Length is 6 characters for password");
            });

        RuleSet(
            UserController.UserValidationsRules.GetUserData.ToString(),
            () =>
            {
                RuleFor(u => u.Id).NotEmpty().WithMessage("Can't Leave Id Empty if Accessing Data");
            }
            );
    }
    
    public (bool Success,List<string> errors) ValidateSingleString_ForPassword(string input)
    {
        var errors = new List<string>();
        var stringValidator = new InlineValidator<string>();
        stringValidator.RuleFor(x => x).NotEmpty().WithMessage("New Password Can't Be Empty");
        stringValidator.RuleFor(x => x).MinimumLength(4).WithMessage("Password Length must be more than 6 characters");

        var validationResult = stringValidator.Validate(input);
        errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
        return (validationResult.IsValid,errors);
    }
}