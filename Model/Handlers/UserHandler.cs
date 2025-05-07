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
    
    public async Task<int> RegisterNewUser(User user)
    {


        return -1;
    }

    public async Task<bool> LoginUser(User user)
    {

        return false;
    }

    public async Task<int> EditUserDetails(User user)
    {
        
        return -1;
    }

    public async Task<List<Income>> GetUserIncomes(User user)
    {

        return new List<Income>();
    }

    public async Task<List<Budget>> GetUserBudgets(User user)
    {

        return new List<Budget>();
    }

    public async Task<List<Expense>> GetUserExpenses(User user)
    {

        return new List<Expense>();
    }
    
    
}