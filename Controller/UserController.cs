using Model.Entities;
using Model.Handlers;
using Model.Utilities;

public class UserController
{
    private readonly UserHandler _userHandler = ServicesContainer.Instance.GetService<UserHandler>();

    public async Task<(bool Success, int UserId)> TryAddUser(User user)
    {
        int result = await _userHandler.RegisterNewUser(user);
        return (result != -1, result);
    }

    public async Task<(bool Success, User? UpdatedUser)> TryUpdateUser(User newValues)
    {
        var updatedUser = await _userHandler.EditUserDetails(newValues);
        return (updatedUser != null, updatedUser);
    }

    public async Task<(bool Success, User? UpdatedUser)> TryChangeUserPassword(string newPassword, User user)
    {
        var result = await _userHandler.ChangeUserPassword(newPassword, user);
        return (result != null, result);
    }
    public async Task<(bool Success, int UserId)> TryLoginUser(User user)
    {
        var result = await _userHandler.LoginUser(user);
        return (result != -1, result);  
    }

    public async Task<(bool Success, List<Income> Incomes)> TryGetUserIncomes(User user)
    {
        var incomes = await _userHandler.GetUserIncomes(user);
        return (incomes.Count > 0, incomes);
    }

    public async Task<(bool Success, List<Budget> Budgets)> TryGetUserBudgets(User user)
    {
        var budgets = await _userHandler.GetUserBudgets(user);
        return (budgets.Count > 0, budgets);
    }

    public async Task<(bool Success, List<Expense> Expenses)> TryGetUserExpenses(User user)
    {
        var expenses = await _userHandler.GetUserExpenses(user);
        return (expenses.Count > 0, expenses);
    }
}