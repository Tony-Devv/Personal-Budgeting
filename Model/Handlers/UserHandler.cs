using Model.Entities;
using Model.Interfaces;
using Model.Utilities;

namespace Model.Handlers
{
    /// <summary>
    /// Handler class responsible for managing user-related operations such as registration, login,
    /// password change, and retrieving user details.
    /// </summary>
    public class UserHandler
    {
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserRepository _userRepository;

        /// <summary>
        /// Initializes a new instance of the <see cref="UserHandler"/> class.
        /// </summary>
        public UserHandler()
        {
            _passwordHasher = ServicesContainer.Instance.GetService<IPasswordHasher>();
            _userRepository = ServicesContainer.Instance.GetService<IUserRepository>();
        }

        /// <summary>
        /// Registers a new user by checking if the user already exists, hashing the password, and saving the user to the repository.
        /// </summary>
        /// <param name="user">The user to be registered.</param>
        /// <returns>
        /// A <see cref="Task{User?}"/> representing the asynchronous operation. The task result is the created <see cref="User"/> 
        /// if the registration is successful, or <c>null</c> if the user already exists.
        /// </returns>
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

        /// <summary>
        /// Logs in a user by verifying the email and password, returning the corresponding user if credentials are valid.
        /// </summary>
        /// <param name="user">The user attempting to log in.</param>
        /// <returns>
        /// A <see cref="Task{User?}"/> representing the asynchronous operation. The task result is the <see cref="User"/> 
        /// if the login is successful, or <c>null</c> if the credentials are invalid.
        /// </returns>
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

        /// <summary>
        /// Edits the details of an existing user.
        /// </summary>
        /// <param name="user">The user whose details need to be updated.</param>
        /// <returns>
        /// A <see cref="Task{User?}"/> representing the asynchronous operation. The task result is the updated <see cref="User"/>, 
        /// or <c>null</c> if the user does not exist.
        /// </returns>
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

        /// <summary>
        /// Retrieves a user by their ID.
        /// </summary>
        /// <param name="id">The ID of the user to retrieve.</param>
        /// <returns>
        /// A <see cref="Task{User?}"/> representing the asynchronous operation. The task result is the <see cref="User"/> 
        /// if found, or <c>null</c> if the user does not exist.
        /// </returns>
        public async Task<User?> GetUserById(int id)
        {
            try
            {
                if (!await _userRepository.CheckExist(id))
                    return null;

                User? user = await _userRepository.GetUserById(id);
                return user;
            }
            catch (Exception e)
            {
                LogError("GetUserById", e);
                return null;
            }
        }

        /// <summary>
        /// Retrieves a user by their email.
        /// </summary>
        /// <param name="email">The email of the user to retrieve.</param>
        /// <returns>
        /// A <see cref="Task{User?}"/> representing the asynchronous operation. The task result is the <see cref="User"/> 
        /// if found, or <c>null</c> if the user does not exist.
        /// </returns>
        public async Task<User?> GetUserByEmail(string email)
        {
            try
            {
                if (!await _userRepository.CheckUserExistsByEmail(email))
                    return null;

                User? user = await _userRepository.RetrieveUserByEmail(email);
                return user;
            }
            catch (Exception e)
            {
                LogError("GetUserByEmail", e);
                return null;
            }
        }

        /// <summary>
        /// Changes the password for a user after verifying the current password.
        /// </summary>
        /// <param name="newPassword">The new password to set.</param>
        /// <param name="user">The user whose password is to be changed.</param>
        /// <returns>
        /// A <see cref="Task{User?}"/> representing the asynchronous operation. The task result is the <see cref="User"/> 
        /// if the password change is successful, or <c>null</c> if the old password is incorrect or the user does not exist.
        /// </returns>
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

        /// <summary>
        /// Retrieves all incomes for a user.
        /// </summary>
        /// <param name="user">The user whose incomes are to be retrieved.</param>
        /// <returns>
        /// A <see cref="Task{List{Income}}"/> representing the asynchronous operation. The task result is a list of <see cref="Income"/> 
        /// if the user exists, or an empty list if the user does not have any incomes.
        /// </returns>
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

        /// <summary>
        /// Retrieves all budgets for a user.
        /// </summary>
        /// <param name="user">The user whose budgets are to be retrieved.</param>
        /// <returns>
        /// A <see cref="Task{List{Budget}}"/> representing the asynchronous operation. The task result is a list of <see cref="Budget"/> 
        /// if the user exists, or an empty list if the user does not have any budgets.
        /// </returns>
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

        /// <summary>
        /// Retrieves all expenses for a user.
        /// </summary>
        /// <param name="user">The user whose expenses are to be retrieved.</param>
        /// <returns>
        /// A <see cref="Task{List{Expense}}"/> representing the asynchronous operation. The task result is a list of <see cref="Expense"/> 
        /// if the user exists, or an empty list if the user does not have any expenses.
        /// </returns>
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

        /// <summary>
        /// Retrieves the total income for a user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>
        /// A <see cref="Task{decimal}"/> representing the asynchronous operation. The task result is the total income 
        /// for the user, or <c>-1</c> if the user does not exist.
        /// </returns>
        public async Task<decimal> GetTotalUserIncomes(int id)
        {
            try
            {
                if (!await _userRepository.CheckExist(id))
                    return -1;

                decimal total = await _userRepository.GetTotalUserIncomes(id);
                return total;
            }
            catch (Exception e)
            {
                LogError("GetTotalUserIncomes", e);
                return -1;
            }
        }

        /// <summary>
        /// Retrieves the total expenses for a user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <returns>
        /// A <see cref="Task{decimal}"/> representing the asynchronous operation. The task result is the total expenses 
        /// for the user, or <c>-1</c> if the user does not exist.
        /// </returns>
        public async Task<decimal> GetTotalUserSpentExpenses(int id)
        {
            try
            {
                if (!await _userRepository.CheckExist(id))
                    return -1;

                decimal total = await _userRepository.GetTotalUserExpenses(id);
                return total;
            }
            catch (Exception e)
            {
                LogError("GetTotalUserSpentExpenses", e);
                return -1;
            }
        }

        /// <summary>
        /// Retrieves the total amount spent on a specific budget by a user.
        /// </summary>
        /// <param name="id">The user ID.</param>
        /// <param name="budgetId">The ID of the budget.</param>
        /// <returns>
        /// A <see cref="Task{decimal}"/> representing the asynchronous operation. The task result is the total amount spent 
        /// on the budget, or <c>-1</c> if the user does not exist.
        /// </returns>
        public async Task<decimal> GetTotalAmountSpentOnBudget(int id, int budgetId)
        {
            try
            {
                if (!await _userRepository.CheckExist(id))
                    return -1;

                decimal total = await _userRepository.GetTotalBudgetSpentAmount(id, budgetId);
                return total;
            }
            catch (Exception e)
            {
                LogError("GetTotalAmountSpentOnBudget", e);
                return -1;
            }
        }

        /// <summary>
        /// Logs errors to the console with context and details.
        /// </summary>
        /// <param name="context">The method or operation where the error occurred.</param>
        /// <param name="e">The exception to log.</param>
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
}
