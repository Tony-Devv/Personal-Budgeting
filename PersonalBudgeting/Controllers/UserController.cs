using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using PersonalBudgeting.Models;

namespace PersonalBudgeting.Controllers
{
    public class UserController
    {
        private readonly List<User> _users = new List<User>(); // Simulated user storage
        
        public UserController()
        {
            // Add some sample users for testing
            _users.Add(new User
            {
                Id = 1,
                UserName = "John Doe",
                Email = "john@example.com",
                Phone = "555-123-4567",
                Password = "password123"
            });
            
            _users.Add(new User
            {
                Id = 2,
                UserName = "Jane Smith",
                Email = "jane@example.com",
                Phone = "555-987-6543",
                Password = "password456"
            });
        }
        
        public async Task<(bool success, User? result, List<string> errors)> TryLogin(string username, string password)
        {
            var errors = new List<string>();
            
            try
            {
                // Validate input
                if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                {
                    errors.Add("Username and password are required");
                    return (false, null, errors);
                }
                
                // Find user by username
                var user = _users.FirstOrDefault(u => 
                    u.UserName.Equals(username, StringComparison.OrdinalIgnoreCase) || 
                    (u.Email != null && u.Email.Equals(username, StringComparison.OrdinalIgnoreCase)));
                
                if (user == null)
                {
                    errors.Add("User not found");
                    return (false, null, errors);
                }
                
                // Validate password
                if (user.Password != password)
                {
                    errors.Add("Incorrect password");
                    return (false, null, errors);
                }
                
                // In a real application, you would use proper password hashing
                await Task.Delay(100); // Simulate network delay
                
                return (true, user, errors);
            }
            catch (Exception ex)
            {
                errors.Add($"Error during login: {ex.Message}");
                return (false, null, errors);
            }
        }
        
        public async Task<(bool success, User? result, List<string> errors)> TryRegister(User user)
        {
            var errors = new List<string>();
            
            try
            {
                // Validate input
                if (user == null)
                {
                    errors.Add("User is null");
                    return (false, null, errors);
                }
                
                if (string.IsNullOrWhiteSpace(user.UserName))
                {
                    errors.Add("Username is required");
                    return (false, null, errors);
                }
                
                if (string.IsNullOrWhiteSpace(user.Password))
                {
                    errors.Add("Password is required");
                    return (false, null, errors);
                }
                
                if (user.Password.Length < 6)
                {
                    errors.Add("Password must be at least 6 characters");
                    return (false, null, errors);
                }
                
                if (!string.IsNullOrWhiteSpace(user.Email) && (!user.Email.Contains("@") || !user.Email.Contains(".")))
                {
                    errors.Add("Invalid email format");
                    return (false, null, errors);
                }
                
                // Check if username or email already exists
                if (_users.Any(u => u.UserName.Equals(user.UserName, StringComparison.OrdinalIgnoreCase)))
                {
                    errors.Add("Username already exists");
                    return (false, null, errors);
                }
                
                if (!string.IsNullOrWhiteSpace(user.Email) && 
                    _users.Any(u => u.Email != null && u.Email.Equals(user.Email, StringComparison.OrdinalIgnoreCase)))
                {
                    errors.Add("Email already exists");
                    return (false, null, errors);
                }
                
                // Generate new ID
                user.Id = _users.Any() ? _users.Max(u => u.Id) + 1 : 1;
                
                // Add user to the collection
                _users.Add(user);
                
                // In a real application, you would hash the password and save to a database
                await Task.Delay(100); // Simulate network delay
                
                return (true, user, errors);
            }
            catch (Exception ex)
            {
                errors.Add($"Error during registration: {ex.Message}");
                return (false, null, errors);
            }
        }
        
        public async Task<(bool success, User? result, List<string> errors)> TryGetUserById(int userId)
        {
            var errors = new List<string>();
            
            try
            {
                var user = _users.FirstOrDefault(u => u.Id == userId);
                
                if (user == null)
                {
                    errors.Add("User not found");
                    return (false, null, errors);
                }
                
                await Task.Delay(100); // Simulate network delay
                
                return (true, user, errors);
            }
            catch (Exception ex)
            {
                errors.Add($"Error getting user: {ex.Message}");
                return (false, null, errors);
            }
        }

        public async Task<(bool success, User? result, List<string> errors)> TryUpdateUser(User user)
        {
            var errors = new List<string>();
            
            try
            {
                // Validate input
                if (user == null)
                {
                    errors.Add("User is null");
                    return (false, null, errors);
                }
                
                if (string.IsNullOrWhiteSpace(user.UserName))
                {
                    errors.Add("Username is required");
                    return (false, null, errors);
                }
                
                if (!string.IsNullOrWhiteSpace(user.Email) && (!user.Email.Contains("@") || !user.Email.Contains(".")))
                {
                    errors.Add("Invalid email format");
                    return (false, null, errors);
                }
                
                // Check if user exists
                var existingUser = _users.FirstOrDefault(u => u.Id == user.Id);
                if (existingUser == null)
                {
                    errors.Add("User not found");
                    return (false, null, errors);
                }
                
                // Update user information
                existingUser.UserName = user.UserName;
                existingUser.Email = user.Email;
                existingUser.Phone = user.Phone;
                
                // Save changes (simulated)
                // In a real application, this would save to a database
                await Task.Delay(100); // Simulate network delay
                
                return (true, existingUser, errors);
            }
            catch (Exception ex)
            {
                errors.Add($"Error updating user: {ex.Message}");
                return (false, null, errors);
            }
        }

        public async Task<(bool success, User? result, List<string> errors)> TryChangePassword(
            int userId, string currentPassword, string newPassword)
        {
            var errors = new List<string>();
            
            try
            {
                // Check if user exists
                var user = _users.FirstOrDefault(u => u.Id == userId);
                if (user == null)
                {
                    errors.Add("User not found");
                    return (false, null, errors);
                }
                
                // Validate current password
                if (user.Password != currentPassword)
                {
                    errors.Add("Current password is incorrect");
                    return (false, null, errors);
                }
                
                // Validate new password
                if (string.IsNullOrWhiteSpace(newPassword))
                {
                    errors.Add("New password cannot be empty");
                    return (false, null, errors);
                }
                
                if (newPassword.Length < 6)
                {
                    errors.Add("New password must be at least 6 characters");
                    return (false, null, errors);
                }
                
                // Update password
                user.Password = newPassword;
                
                // Save changes (simulated)
                // In a real application, this would save to a database
                await Task.Delay(100); // Simulate network delay
                
                return (true, user, errors);
            }
            catch (Exception ex)
            {
                errors.Add($"Error changing password: {ex.Message}");
                return (false, null, errors);
            }
        }
    }
} 