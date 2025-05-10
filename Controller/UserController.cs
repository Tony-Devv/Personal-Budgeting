using System.ComponentModel.DataAnnotations;
using Controller.Validators;
using FluentValidation;
using FluentValidation.Internal;
using Model.Entities;
using Model.Handlers;
using Model.Utilities;

public class UserController
{
    public enum UserValidationsRules
    {
        Register,
        Login,
        Edit,
        ChangePassword,
        GetUserData
    }
    
    private readonly UserHandler _userHandler = ServicesContainer.Instance.GetService<UserHandler>();
    private readonly UserInputValidator _userInputValidator = new UserInputValidator();

    
    public async Task<(bool Success, User ?User,List<string> errorMessages)> TryAddUser(User user) // valid (UserName,Email,Password,PhoneNumber)
    {
        var context = CreateContext(user, UserValidationsRules.Register);
        
        var validationResult = await _userInputValidator.ValidateAsync(context);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return (false, null, errors);
        }
        
        User? result = await _userHandler.RegisterNewUser(user);
        return (result != null, result,new List<string>());
    }

    public async Task<(bool Success, User? user, List<string> errorMessages)> TryGetUserById(int id)
    {
        User? result = await _userHandler.GetUserById(id);
        return (result != null, result, new List<string>());
    }
    
    
    public async Task<(bool Success, User? user, List<string> errorMessages)> TryGetUserByEmail(string email)
    {
        User? result = await _userHandler.GetUserByEmail(email);
        return (result != null, result, new List<string>());
    }

    public async Task<(bool Success, User? UpdatedUser,List<string> errors)> TryUpdateUser(User newValues)
    {
        var context = CreateContext(newValues, UserValidationsRules.Edit);
        
        var validationResult = await _userInputValidator.ValidateAsync(context);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return (false, null, errors);
        }
        
        
        var updatedUser = await _userHandler.EditUserDetails(newValues);
        return (updatedUser != null, updatedUser,new List<string>());
    }

    public async Task<(bool Success, User? UpdatedUser,List<string> errors)> TryChangeUserPassword(string newPassword, User user)
    {
        var context = CreateContext(user, UserValidationsRules.ChangePassword);
        
        var validationResult = await _userInputValidator.ValidateAsync(context);

        var newPasswordValidationResult = _userInputValidator.ValidateSingleString_ForPassword(newPassword);
        
        if (!validationResult.IsValid || newPasswordValidationResult.Success)
        {
            var model_errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            
            model_errors.AddRange(newPasswordValidationResult.errors);

            return (false, null, model_errors);
        }
        
        var result = await _userHandler.ChangeUserPassword(newPassword, user);
        return (result != null, result, new List<string>());
    }
    public async Task<(bool Success, User ? User,List<string> errors)> TryLoginUser(User user)
    {

        var context = CreateContext(user, UserValidationsRules.Login);
        
        var validationResult = await _userInputValidator.ValidateAsync(context);
        
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return (false, null, errors);
        }
        
        var result = await _userHandler.LoginUser(user);
        return (result != null, result,new List<string>());  
    }

    public async Task<(bool Success, List<Income> Incomes,List<string> errors)> TryGetUserIncomes(User user)
    {
        var context = CreateContext(user, UserValidationsRules.GetUserData);

        var validationResult = await _userInputValidator.ValidateAsync(context);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return (false, new List<Income>(), errors);
        }
        
        var incomes = await _userHandler.GetUserIncomes(user);
        return (incomes.Count > 0, incomes, new List<string>());
    }

    public async Task<(bool Success, List<Budget> Budgets,List<string> errors)> TryGetUserBudgets(User user)
    {
        var context = CreateContext(user, UserValidationsRules.GetUserData);

        var validationResult = await _userInputValidator.ValidateAsync(context);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return (false, new List<Budget>(), errors);
        }
        
        var budgets = await _userHandler.GetUserBudgets(user);
        return (budgets.Count > 0, budgets,new List<string>());
    }

    public async Task<(bool Success, List<Expense> Expenses,List<string> errors)> TryGetUserExpenses(User user)
    {
        var context = CreateContext(user, UserValidationsRules.GetUserData);

        var validationResult = await _userInputValidator.ValidateAsync(context);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return (false, new List<Expense>(), errors);
        }
        
        var expenses = await _userHandler.GetUserExpenses(user);
        return (expenses.Count > 0, expenses,new List<string>());
    }


    public async Task<(bool Success, decimal Total, List<string> Errors)> TryGetTotalUserIncomes(int userId)
    {
        decimal total = await _userHandler.GetTotalUserIncomes(userId);
        return (total >= 0, total, new List<string>());
    }
    
    public async Task<(bool Success, decimal Total, List<string> Errors)> TryGetTotalUserSpentExpenses(int userId)
    {
        decimal total = await _userHandler.GetTotalUserSpentExpenses(userId);
        return (total >= 0, total, new List<string>());
    }
    
    public async Task<(bool Success, decimal Total, List<string> Errors)> TryGetTotalAmountSpentOnBudget(int userId, int budgetId)
    {
        decimal total = await _userHandler.GetTotalAmountSpentOnBudget(userId, budgetId);
        return (total >= 0, total, new List<string>());
    }



    

    private ValidationContext<User> CreateContext(User user, UserValidationsRules validationsRules)
    {
        var context = new ValidationContext<User>(user,new PropertyChain(),
            new RulesetValidatorSelector(new[] { validationsRules.ToString() }));

        return context;
    } 
}