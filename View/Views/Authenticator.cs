using Model.Entities;
using View;
using Controller;
using View.Views;

public class Authenticator : IView
{
    private UserController _userController;
    private IView _dashBoard;
    
    public Task Initialize(User obj)
    {
        _userController = new UserController();
        _dashBoard = new DashBoard();
        return Task.CompletedTask;
    }

    public async Task Show()
    {
        while (true)
        {
            User ? MoveToDashBoard = null;
            Console.Clear();
            Console.WriteLine("Welcome! Please choose an option:");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Sign up");
            Console.WriteLine("3. Exit");
            Console.Write("Enter your choice: ");

            var input = Console.ReadLine();
            switch (input)
            {
                case "1":
                    MoveToDashBoard = await LoginAsync();
                    break;
                case "2":
                    MoveToDashBoard = await RegisterAsync();
                    break;
                case "3":
                    Console.WriteLine("Exiting...");
                    return;
                default:
                    Console.WriteLine("Invalid option. Press Enter to try again...");
                    Console.ReadLine();
                    break;
            }

            if (MoveToDashBoard != null)
            {
                await _dashBoard.Initialize(MoveToDashBoard);
                await _dashBoard.Show();
                break;
            }
        }
    }

    private async Task<User?> LoginAsync()
    {
        Console.Clear();
        Console.WriteLine("== Login ==");

        var user = new User();

        Console.Write("Email: ");
        user.Email = Console.ReadLine();

        Console.Write("Password: ");
        user.Password = Console.ReadLine();

        var result = await _userController.TryLoginUser(user);

        if (result.Success)
        {
            Console.WriteLine($"Login successful. Welcome, {result.User.UserName}!");
            return result.User;
        }
        
        Console.WriteLine("Login failed:");
        foreach (var error in result.errors)
            Console.WriteLine($"- {error}");

        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
        return null;
    }

    private async Task<User?> RegisterAsync()
    {
        Console.Clear();
        Console.WriteLine("== Sign up ==");

        var user = new User();

        Console.Write("User name: ");
        user.UserName = Console.ReadLine();

        Console.Write("Email: ");
        user.Email = Console.ReadLine();

        Console.Write("Password: ");
        user.Password = Console.ReadLine();

        Console.Write("Phone number: ");
        user.PhoneNumber = Console.ReadLine();

        var result = await _userController.TryAddUser(user);

        if (result.Success)
        {
            Console.WriteLine("Registration successful!");
            return result.User;
        }
        Console.WriteLine("Registration failed:");
        foreach (var error in result.errorMessages)
            Console.WriteLine($"- {error}");

        Console.WriteLine("Press Enter to continue...");
        Console.ReadLine();
        return null;
    }
}
