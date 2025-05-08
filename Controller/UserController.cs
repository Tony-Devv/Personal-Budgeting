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

    public async Task<(bool Success, int UserId)> TryUpdateUser(int userId, User newValues)
    {
        var updatedUser = await _userHandler.EditUserDetails(userId, newValues);
        return (updatedUser != null, updatedUser);
    }



    public async Task<bool> TrySearchUserByEmail(string email)
    {
        var user = await _userHandler.RetrieveUserByEmail(email);
        return user != null;
    }

}
