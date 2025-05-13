using System.ComponentModel.DataAnnotations;
using Controller.Validators;
using FluentValidation;
using FluentValidation.Internal;
using Model.Entities;
using Model.Handlers;
using Model.Utilities;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json;

/// <summary>
/// The UserController class handles various user-related operations, such as registration, login, updating user details, 
/// changing passwords, and retrieving user data. It leverages a handler for business logic and validators for user inputs.
/// </summary>
public class UserController
{
    /// <summary>
    /// Defines validation rules for different user-related operations.
    /// </summary>
    public enum UserValidationsRules
    {
        /// <summary> Validation rule for registering a new user. </summary>
        Register,
        /// <summary> Validation rule for logging in an existing user. </summary>
        Login,
        /// <summary> Validation rule for editing user details. </summary>
        Edit,
        /// <summary> Validation rule for changing a user's password. </summary>
        ChangePassword,
        /// <summary> Validation rule for retrieving user data. </summary>
        GetUserData
    }

    private readonly UserHandler _userHandler = ServicesContainer.Instance.GetService<UserHandler>();
    private readonly UserInputValidator _userInputValidator = new UserInputValidator();

    /// <summary>
    /// Attempts to add a new user after validating the user's input.
    /// </summary>
    /// <param name="user">The user details to be added.</param>
    /// <returns>A tuple containing the success status, the user object (if successful), and a list of error messages (if any).</returns>
    public async Task<(bool Success, User ?User, List<string> errorMessages)> TryAddUser(User user) // valid (UserName,Email,Password,PhoneNumber)
    {
        var context = CreateContext(user, UserValidationsRules.Register);
        
        var validationResult = await _userInputValidator.ValidateAsync(context);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return (false, null, errors);
        }
        
        User? result = await _userHandler.RegisterNewUser(user);
        return (result != null, result, new List<string>());
    }

    /// <summary>
    /// Attempts to retrieve a user by their ID.
    /// </summary>
    /// <param name="id">The user's unique identifier.</param>
    /// <returns>A tuple containing the success status, the user object (if found), and any error messages.</returns>
    public async Task<(bool Success, User? user, List<string> errorMessages)> TryGetUserById(int id)
    {
        User? result = await _userHandler.GetUserById(id);
        return (result != null, result, new List<string>());
    }

    /// <summary>
    /// Attempts to retrieve a user by their email address.
    /// </summary>
    /// <param name="email">The user's email address.</param>
    /// <returns>A tuple containing the success status, the user object (if found), and any error messages.</returns>
    public async Task<(bool Success, User? user, List<string> errorMessages)> TryGetUserByEmail(string email)
    {
        User? result = await _userHandler.GetUserByEmail(email);
        return (result != null, result, new List<string>());
    }

    /// <summary>
    /// Attempts to update an existing user's details after validating the input.
    /// </summary>
    /// <param name="newValues">The updated user details.</param>
    /// <returns>A tuple containing the success status, the updated user object (if successful), and any error messages.</returns>
    public async Task<(bool Success, User? UpdatedUser, List<string> errors)> TryUpdateUser(User newValues)
    {
        var context = CreateContext(newValues, UserValidationsRules.Edit);
        
        var validationResult = await _userInputValidator.ValidateAsync(context);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return (false, null, errors);
        }
        
        var updatedUser = await _userHandler.EditUserDetails(newValues);
        return (updatedUser != null, updatedUser, new List<string>());
    }

    /// <summary>
    /// Attempts to change a user's password after validating the input.
    /// </summary>
    /// <param name="newPassword">The new password.</param>
    /// <param name="user">The user whose password is to be changed.</param>
    /// <returns>A tuple containing the success status, the updated user object (if successful), and any error messages.</returns>
    public async Task<(bool Success, User? UpdatedUser, List<string> errors)> TryChangeUserPassword(string newPassword, User user)
    {
        var context = CreateContext(user, UserValidationsRules.ChangePassword);
        
        var validationResult = await _userInputValidator.ValidateAsync(context);

        var newPasswordValidationResult = _userInputValidator.ValidateSingleString_ForPassword(newPassword);
        
        if (!validationResult.IsValid || !newPasswordValidationResult.Success)
        {
            var model_errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            
            model_errors.AddRange(newPasswordValidationResult.Errors);

            return (false, null, model_errors);
        }
        
        var result = await _userHandler.ChangeUserPassword(newPassword, user);
        return (result != null, result, new List<string>());
    }

    /// <summary>
    /// Attempts to log in a user after validating the input.
    /// </summary>
    /// <param name="user">The user attempting to log in.</param>
    /// <returns>A tuple containing the success status, the user object (if successful), and any error messages.</returns>
    public async Task<(bool Success, User? User, List<string> errors)> TryLoginUser(User user)
    {
        var context = CreateContext(user, UserValidationsRules.Login);
        
        var validationResult = await _userInputValidator.ValidateAsync(context);
        
        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return (false, null, errors);
        }
        
        var result = await _userHandler.LoginUser(user);
        return (result != null, result, new List<string>());
    }

    /// <summary>
    /// Attempts to retrieve a list of the user's incomes after validating the input.
    /// </summary>
    /// <param name="user">The user whose incomes are to be retrieved.</param>
    /// <returns>A tuple containing the success status, a list of the user's incomes, and any error messages.</returns>
    public async Task<(bool Success, List<Income> Incomes, List<string> errors)> TryGetUserIncomes(User user)
    {
        var context = CreateContext(user, UserValidationsRules.GetUserData);

        var validationResult = await _userInputValidator.ValidateAsync(context);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return (false, new List<Income>(), errors);
        }
        
        var incomes = await _userHandler.GetUserIncomes(user);
        return (incomes.Count >= 0, incomes, new List<string>());
    }

    /// <summary>
    /// Attempts to retrieve a list of the user's budgets after validating the input.
    /// </summary>
    /// <param name="user">The user whose budgets are to be retrieved.</param>
    /// <returns>A tuple containing the success status, a list of the user's budgets, and any error messages.</returns>
    public async Task<(bool Success, List<Budget> Budgets, List<string> errors)> TryGetUserBudgets(User user)
    {
        var context = CreateContext(user, UserValidationsRules.GetUserData);

        var validationResult = await _userInputValidator.ValidateAsync(context);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return (false, new List<Budget>(), errors);
        }
        
        var budgets = await _userHandler.GetUserBudgets(user);
        return (budgets.Count >= 0, budgets, new List<string>());
    }

    /// <summary>
    /// Attempts to retrieve a list of the user's expenses after validating the input.
    /// </summary>
    /// <param name="user">The user whose expenses are to be retrieved.</param>
    /// <returns>A tuple containing the success status, a list of the user's expenses, and any error messages.</returns>
    public async Task<(bool Success, List<Expense> Expenses, List<string> errors)> TryGetUserExpenses(User user)
    {
        var context = CreateContext(user, UserValidationsRules.GetUserData);

        var validationResult = await _userInputValidator.ValidateAsync(context);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors.Select(e => e.ErrorMessage).ToList();
            return (false, new List<Expense>(), errors);
        }
        
        var expenses = await _userHandler.GetUserExpenses(user);
        return (expenses.Count >= 0, expenses, new List<string>());
    }

    /// <summary>
    /// Attempts to retrieve the total amount of income for a user.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <returns>A tuple containing the success status, the total income, and any error messages.</returns>
    public async Task<(bool Success, decimal Total, List<string> Errors)> TryGetTotalUserIncomes(int userId)
    {
        decimal total = await _userHandler.GetTotalUserIncomes(userId);
        return (total >= 0, total, new List<string>());
    }

    /// <summary>
    /// Attempts to retrieve the total amount of expenses spent by a user.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <returns>A tuple containing the success status, the total expenses, and any error messages.</returns>
    public async Task<(bool Success, decimal Total, List<string> Errors)> TryGetTotalUserSpentExpenses(int userId)
    {
        decimal total = await _userHandler.GetTotalUserSpentExpenses(userId);
        return (total >= 0, total, new List<string>());
    }

    /// <summary>
    /// Attempts to retrieve the total amount spent by a user on a specific budget.
    /// </summary>
    /// <param name="userId">The user's unique identifier.</param>
    /// <param name="budgetId">The unique identifier for the budget.</param>
    /// <returns>A tuple containing the success status, the total spent, and any error messages.</returns>
    public async Task<(bool Success, decimal Total, List<string> Errors)> TryGetTotalAmountSpentOnBudget(int userId, int budgetId)
    {
        decimal total = await _userHandler.GetTotalAmountSpentOnBudget(userId, budgetId);
        return (total >= 0, total, new List<string>());
    }

    /// <summary>
    /// Creates a validation context for a given user and validation rule.
    /// </summary>
    /// <param name="user">The user to be validated.</param>
    /// <param name="validationsRules">The validation rules to be applied.</param>
    /// <returns>A validation context with the specified rules for the user.</returns>
    private ValidationContext<User> CreateContext(User user, UserValidationsRules validationsRules)
    {
        var context = new ValidationContext<User>(user, new PropertyChain(), 
            new RulesetValidatorSelector(new[] { validationsRules.ToString() }));

        return context;
    }
}

public class ReminderService
{
    private const string ApiKey = "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";
    private const string FromEmail = "AntonDataStructure@gmail.com";

    public static async Task SendExpenseReminderAsync(User user, Expense expense)
    {
        if (user == null || expense == null || string.IsNullOrWhiteSpace(user.Email) || expense.ReminderTime == null)
        {
            Console.WriteLine("ReminderService: Missing user, expense, email, or reminder date.");
            return;
        }

        var payload = new
        {
            personalizations = new[]
            {
                new { to = new[] { new { email = user.Email } } }
            },
            from = new { email = FromEmail },
            subject = "Expense Reminder",
            content = new[]
            {
                new
                {
                    type = "text/plain",
                    value = $"Hello {user.UserName},\n\n" +
                            $"This is a reminder for your expense \"{expense.ExpenseName}\".\n" +
                            $"Required Amount: {expense.RequiredAmount:C}\n" +
                            $"Spent Amount: {expense.SpentAmount:C}\n" +
                            $"Reminder Date: {expense.ReminderTime:yyyy-MM-dd}\n\n" +
                            "Best Regards,\nPersonal Budgeting Team"
                }
            }
        };

        string jsonPayload = JsonSerializer.Serialize(payload);

        Console.WriteLine("ReminderService: JSON Payload: " + jsonPayload);

        using var client = new HttpClient();
        client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
        var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
        var response = await client.PostAsync("https://api.sendgrid.com/v3/mail/send", content);

        Console.WriteLine("ReminderService: Response status: " + response.StatusCode);
        string responseBody = await response.Content.ReadAsStringAsync();
        Console.WriteLine("ReminderService: Response body: " + responseBody);
    }
}
