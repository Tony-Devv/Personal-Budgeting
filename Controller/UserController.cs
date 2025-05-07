using Model.Entities;
using Model.Handlers;
using Model.Utilities;

public class UserController
{
    private readonly UserHandler _userHandler = ServicesContainer.Instance.GetService<UserHandler>();

    public async Task<(bool Success,int UserId)> TryRegisterUser(User user)
    {
        int result = await _userHandler.RegisterNewUser(user);
        return (result != -1,result);
    }
    public bool TryLoginUser(User user) { return true; }
    public bool TryEditUser(User user) { return true; }
    public bool TryGetIncomes(User user) { return true; }
    public bool TryGetBudgets(User user) { return true; }
    public bool TryGetUserExpenses(User user) { return true; }
}

