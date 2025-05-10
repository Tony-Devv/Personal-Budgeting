using Model.Entities;
using Model.Interfaces;
using Model.Utilities;

namespace Model.Handlers;

public class UserHandler
{
    private readonly IPasswordHasher _passwordHasher;
    private readonly IUserRepository _userRepository;

    public UserHandler()
    {
        _passwordHasher = ServicesContainer.Instance.GetService<IPasswordHasher>();
        _userRepository = ServicesContainer.Instance.GetService<IUserRepository>();
    }

    public async Task<User?> RegisterNewUser(User user)
    {
        try
        {
            if (await _userRepository.CheckUserExistsByEmail(user.Email))
                return null;

            user.Password = await _passwordHasher.Hash(user.Password);
            await _userRepository.Add(user);
        }
        catch (Exception e)
        {
            LogError("RegisterNewUser", e);
        }

        return await _userRepository.GetById(user.Id)!;
    }

    public async Task<User?> LoginUser(User user)
    {
        User? result = null;

        try
        {
            User? u = await _userRepository.RetrieveUserByEmail(user.Email);
            if (u != null && await _passwordHasher.Verify(u.Password, user.Password))
            {
                result = await _userRepository.GetById(u.Id)!;
            }
        }
        catch (Exception e)
        {
            LogError("LoginUser", e);
        }

        return result;
    }

    public async Task<User?> EditUserDetails(User user)
    {
        try
        {
            if (!await _userRepository.CheckExist(user.Id))
                return null;

            await _userRepository.Update(user);
            return user;
        }
        catch (Exception e)
        {
            LogError("EditUserDetails", e);
            return null;
        }
    }


    public async Task<User?> GetUserById(int id)
    {
        try
        {
            if (!await _userRepository.CheckExist(id))
                return null;

            User ? user = await _userRepository.GetUserById(id);
            return user;
        }
        catch (Exception e)
        {
            LogError("GetUserById", e);
            return null;
        }
    }
    
    
    public async Task<User?> GetUserByEmail(string email)
    {
        try
        {
            if (!await _userRepository.CheckUserExistsByEmail(email))
                return null;

            User ? user = await _userRepository.RetrieveUserByEmail(email);
            return user;
        }
        catch (Exception e)
        {
            LogError("GetUserByEmail", e);
            return null;
        }
    }
    

    public async Task<User?> ChangeUserPassword(string newPassword, User user)
    {
        try
        {
            if (!await _userRepository.CheckUserExistsByEmail(user.Email))
                return null;

            var retrievedUser = await _userRepository.RetrieveUserByEmail(user.Email);
            if (!await _passwordHasher.Verify(retrievedUser!.Password, user.Password))
                return null;

            user.Password = await _passwordHasher.Hash(newPassword);
            
            await _userRepository.Update(user);
            return user;
        }
        catch (Exception e)
        {
            LogError("ChangeUserPassword", e);
            return null;
        }
    }

    public async Task<List<Income>> GetUserIncomes(User user)
    {
        List<Income> result = new();

        try
        {
            if (!await _userRepository.CheckExist(user.Id))
                return result;

            result = (List<Income>)await _userRepository.GetUserIncomes(user);
        }
        catch (Exception e)
        {
            LogError("GetUserIncomes", e);
        }

        return result;
    }

    public async Task<List<Budget>> GetUserBudgets(User user)
    {
        List<Budget> result = new();

        try
        {
            if (!await _userRepository.CheckExist(user.Id))
                return result;

            result = (List<Budget>)await _userRepository.GetUserBudgets(user);
        }
        catch (Exception e)
        {
            LogError("GetUserBudgets", e);
        }

        return result;
    }

    public async Task<List<Expense>> GetUserExpenses(User user)
    {
        List<Expense> result = new();

        try
        {
            if (!await _userRepository.CheckExist(user.Id))
                return result;

            result = (List<Expense>)await _userRepository.GetUserExpenses(user);
        }
        catch (Exception e)
        {
            LogError("GetUserExpenses", e);
        }

        return result;
    }


    public async Task<decimal> GetTotalUserIncomes(int id)
    {
        try
        {
            if (!await _userRepository.CheckExist(id))
                return -1;

            decimal  total = await _userRepository.GetTotalUserIncomes(id);
            return total;
        }
        catch (Exception e)
        {
            LogError("GetTotalUserIncomes", e);
            return -1;
        }
    }
    
    public async Task<decimal> GetTotalUserSpentExpenses(int id)
    {
        try
        {
            if (!await _userRepository.CheckExist(id))
                return -1;

            decimal  total = await _userRepository.GetTotalUserExpenses(id);
            return total;
        }
        catch (Exception e)
        {
            LogError("GetTotalUserSpentExpenses", e);
            return -1;
        }
    }

    
    public async Task<decimal> GetTotalAmountSpentOnBudget(int id,int budgetId)
    {
        try
        {
            if (!await _userRepository.CheckExist(id))
                return -1;

            decimal  total = await _userRepository.GetTotalBudgetSpentAmount(id,budgetId);
            return total;
        }
        catch (Exception e)
        {
            LogError("GetTotalAmountSpentOnBudget", e);
            return -1;
        }
    }


    private void LogError(string context, Exception e)
    {
        var originalColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Error occurred at {context}");
        Console.ForegroundColor = originalColor;

        Console.WriteLine($"Error: {e.Message}");
        Console.WriteLine($"Stack: {e.StackTrace}");
    }
}
