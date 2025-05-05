using Model.Entities;
using Model.Interfaces;

namespace Model.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    public Task<User> RetrieveUserByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CheckUserExistsByEmail(string email)
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<Income>> GetUserIncomes(User user)
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<Budget>> GetUserBudgets(User user)
    {
        throw new NotImplementedException();
    }

    public Task<ICollection<Expense>> GetUserExpenses(User user)
    {
        throw new NotImplementedException();
    }
}