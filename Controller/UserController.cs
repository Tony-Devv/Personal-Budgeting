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
    private const string ApiKey = "USE YOUR OWN SEND GRID API";
    private const string FromEmail = "USE YOUR VERIFIED SEND GRID ACCOUNT";
    private const string SenderName = "Personal Budget Team";

    public static async Task<bool> SendExpenseReminderAsync(User user, Expense expense)
    {
        try
        {
            // Validate inputs before proceeding
            if (string.IsNullOrEmpty(user.Email))
            {
                return false;
            }
            
            // Calculate reminder details
            decimal remainingAmount = expense.RequiredAmount - expense.SpentAmount;
            string remainingClass = remainingAmount <= 0 ? "budget-success" : "budget-warning";
            
            // Calculate progress percentage (avoid division by zero)
            int progressPercentage = expense.RequiredAmount > 0 
                ? (int)Math.Min(100, (expense.SpentAmount / expense.RequiredAmount) * 100) 
                : 0;
            
            // Extract category if available
            string expenseName = expense.ExpenseName;
            if (expense.ExpenseName.Contains(" - "))
            {
                string[] parts = expense.ExpenseName.Split(new string[] { " - " }, StringSplitOptions.None);
                if (parts.Length >= 2)
                {
                    // Use the part after the hyphen
                    expenseName = parts[1].Trim();
                }
            }
            
            // Create HTML message using the template
            string message = $@"<!DOCTYPE html>
<html>
<head>
  <meta charset=""utf-8"">
  <meta name=""viewport"" content=""width=device-width, initial-scale=1"">
  <title>Personal Budget Manager Notification</title>
  <style>
    body {{ font-family: 'Segoe UI', Arial, sans-serif; line-height: 1.6; color: #333333; background-color: #f5f5f5; margin: 0; padding: 0; }}
    .container {{ max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 12px; overflow: hidden; box-shadow: 0 4px 8px rgba(0, 0, 0, 0.1); }}
    .header {{ background-color: #4B6EAF; padding: 24px; text-align: center; color: #ffffff; }}
    .emoji-logo {{ font-size: 48px; margin: 0 auto 12px; }}
    .header h1 {{ margin: 0; font-size: 24px; font-weight: 600; }}
    .content {{ padding: 32px 24px; }}
    .content-title {{ font-size: 20px; font-weight: 600; margin-top: 0; margin-bottom: 20px; color: #4B6EAF; }}
    .budget-card {{ border: 1px solid #e0e0e0; border-radius: 8px; padding: 20px; margin-bottom: 24px; background-color: #fafafa; }}
    .budget-card-title {{ font-size: 18px; font-weight: 500; margin-top: 0; margin-bottom: 16px; color: #333333; }}
    .budget-info {{ display: flex; justify-content: space-between; margin-bottom: 16px; }}
    .budget-label {{ color: #666666; font-size: 14px; }}
    .budget-value {{ font-weight: 600; font-size: 16px; }}
    .budget-danger {{ color: #E74C3C; }}
    .budget-success {{ color: #2ECC71; }}
    .budget-warning {{ color: #F39C12; }}
    .progress-bar-container {{ height: 8px; background-color: #e0e0e0; border-radius: 4px; overflow: hidden; margin-bottom: 16px; }}
    .progress-bar {{ height: 100%; background-color: #4B6EAF; border-radius: 4px; }}
    .btn {{ display: inline-block; background-color: #4B6EAF; color: #ffffff !important; padding: 12px 24px; text-decoration: none; border-radius: 6px; font-weight: 500; text-align: center; }}
    .btn-center {{ display: block; margin: 0 auto; max-width: 200px; }}
    .footer {{ background-color: #f5f5f5; padding: 20px; text-align: center; font-size: 12px; color: #666666; border-top: 1px solid #e0e0e0; }}
    .footer p {{ margin: 5px 0; }}
    .footer-link {{ color: #4B6EAF; text-decoration: none; }}
  </style>
</head>
<body>
  <div class=""container"">
    <div class=""header"">
      <div class=""emoji-logo"">💰</div>
      <h1>Personal Budget Manager</h1>
    </div>
    <div class=""content"">
      <h2 class=""content-title"">Expense Reminder</h2>
      <p>Hello {user.UserName},</p>
      <p>This is a reminder about an upcoming expense in your budget.</p>
      <div class=""budget-card"">
        <h3 class=""budget-card-title"">{expense.ExpenseName}</h3>
        <div class=""budget-info"">
          <span class=""budget-label"">Required Amount:</span>
          <span class=""budget-value"">${expense.RequiredAmount:0.00}</span>
        </div>
        <div class=""budget-info"">
          <span class=""budget-label"">Spent Amount:</span>
          <span class=""budget-value"">${expense.SpentAmount:0.00}</span>
        </div>
        <div class=""budget-info"">
          <span class=""budget-label"">Remaining:</span>
          <span class=""budget-value {remainingClass}"">${remainingAmount:0.00}</span>
        </div>
        <div class=""progress-bar-container"">
          <div class=""progress-bar"" style=""width: {progressPercentage}%""></div>
        </div>
        <div class=""budget-info"">
          <span class=""budget-label"">Progress:</span>
          <span class=""budget-value"">{progressPercentage}%</span>
        </div>
        <div class=""budget-info"">
          <span class=""budget-label"">Due date:</span>
          <span class=""budget-value"">{expense.ReminderTime:yyyy-MM-dd}</span>
        </div>
      </div>
      <p>Stay on top of your finances and manage your expenses efficiently.</p>
      <a href=""https://github.com/Tony-Devv/Personal-Budgeting"" class=""btn btn-center"" style=""color: #ffffff !important;"">View Project on GitHub</a>
    </div>
    <div class=""footer"">
      <p>{SenderName}</p>
      <p>© 2025 Personal Budget Manager</p>
      <p><small>To manage reminder settings, please visit the expense page and edit your reminder.</small></p>
      <p><a href=""https://github.com/Tony-Devv/Personal-Budgeting"" target=""_blank"" class=""footer-link"">View on GitHub</a></p>
    </div>
  </div>
</body>
</html>";
            
            var payload = new
            {
                personalizations = new[]
                {
                    new { to = new[] { new { email = user.Email } } }
                },
                from = new { email = FromEmail, name = SenderName },
                subject = $"Expense Reminder: {expense.ExpenseName}",
                content = new[]
                {
                    new
                    {
                        type = "text/html",
                        value = message
                    }
                }
            };

            string jsonPayload = JsonSerializer.Serialize(payload);
            
            // Send the email using SendGrid API
            using var client = new HttpClient();
            client.DefaultRequestHeaders.Add("Authorization", $"Bearer {ApiKey}");
            client.DefaultRequestHeaders.Add("Accept", "application/json");
            client.Timeout = TimeSpan.FromSeconds(30);
            
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");
            var response = await client.PostAsync("https://api.sendgrid.com/v3/mail/send", content);
            
            return response.IsSuccessStatusCode;
        }
        catch (Exception)
        {
            return false;
        }
    }
}
