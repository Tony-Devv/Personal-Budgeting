using Controller;
using Model.Entities;

namespace View.Views;

public class EditDetailsView : IView
{
    private UserController _userController = new UserController();
    private User _currentUser;

    public Task Initialize(User user)
    {
        _currentUser = user;
        _currentUser.Password = String.Empty;
        return Task.CompletedTask;
    }

    public async Task Show()
    {
        bool running = true;
        while (running)
        {
            Console.Clear();
            Console.WriteLine("=== Edit User Details ===");
            Console.WriteLine("1. Edit User Info");
            Console.WriteLine("2. Change Password");
            Console.WriteLine("0. Back");
            Console.Write("Select an option: ");
            string? choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    await EditUserInfo();
                    break;
                case "2":
                    await ChangePassword();
                    break;
                case "0":
                    running = false;
                    break;
                default:
                    Console.WriteLine("Invalid option.");
                    Pause();
                    break;
            }
        }
    }

    private async Task EditUserInfo()
    {
        Console.Clear();
        Console.WriteLine("=== Edit User Info ===");

        Console.WriteLine($"Current Username: {_currentUser.UserName}");
        Console.Write("New Username (Leave Plank if u don't want to change): ");
        string? newUserName = Console.ReadLine();

        Console.WriteLine($"Current Email: {_currentUser.Email}");
        Console.Write("New Email(Leave Plank if u don't want to change): ");
        string? newEmail = Console.ReadLine();

        Console.WriteLine($"Current Phone Number (Leave Plank if u don't want to change): {_currentUser.PhoneNumber}");
        Console.Write("New Phone Number: ");
        string? newPhone = Console.ReadLine();

        _currentUser.UserName = string.IsNullOrWhiteSpace(newUserName) ? _currentUser.UserName : newUserName;
        _currentUser.Email = string.IsNullOrWhiteSpace(newEmail) ? _currentUser.Email : newEmail;
        _currentUser.PhoneNumber = string.IsNullOrWhiteSpace(newPhone) ? _currentUser.PhoneNumber : newPhone;

        var (success, updatedUser, errors) = await _userController.TryUpdateUser(_currentUser);

        if (success && updatedUser != null)
        {
            _currentUser = updatedUser;
            Console.WriteLine("User info updated successfully.");
        }
        else
        {
            Console.WriteLine("Failed to update user info:");
            foreach (var error in errors)
            {
                Console.WriteLine($"- {error}");
            }
        }

        Pause();
    }

    private async Task ChangePassword()
    {
        Console.Clear();
        Console.WriteLine("=== Change Password ===");

        Console.Write("Enter current password: ");
        string? oldPassword = Console.ReadLine();

        Console.Write("Enter new password: ");
        string? newPassword = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(newPassword) || string.IsNullOrWhiteSpace(oldPassword))
        {
            Console.WriteLine("Passwords cannot be empty.");
            Pause();
            return;
        }

        _currentUser.Password = oldPassword;
        
        // Pass both old and new password to the controller
        var (success, updatedUser, errors) = await _userController.TryChangeUserPassword(newPassword, _currentUser);

        if (success && updatedUser != null)
        {
            _currentUser = updatedUser;
            Console.WriteLine("Password changed successfully.");
        }
        else
        {
            Console.WriteLine("Failed to change password:");
            foreach (var error in errors)
            {
                Console.WriteLine($"- {error}");
            }
        }

        Pause();
    }


    private void Pause()
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }
}
