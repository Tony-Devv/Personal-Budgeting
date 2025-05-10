using Model.Entities;

namespace Model.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> RetrieveUserByEmail(string email);

    Task<User?> GetUserById(int id);
    
    Task<bool> CheckUserExistsByEmail(string email);

    Task<ICollection<Income>> GetUserIncomes(User user);

    Task<ICollection<Budget>> GetUserBudgets(User user);
    
    Task<ICollection<Expense>> GetUserExpenses(User user);
}