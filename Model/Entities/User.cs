using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities;

/// <summary>
/// Represents a user entity in the system.
/// It's being used by EntityFramework to represent the user records.
/// </summary>
/// <remarks>
/// This class maps to the "User" table in the database and maintains
/// relationships with Budget, Expense, and Income entities.
/// </remarks>
[Table("User")]
public class User
{
    /// <summary>
    /// Gets or sets the unique identifier for the user.
    /// </summary>
    /// <value>An integer that uniquely identifies the user.</value>
    /// <remarks>
    /// Can't be 0 or negative if you're performing update, delete, or read operations.
    /// </remarks>
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the username of the user.
    /// </summary>
    /// <value>The username used for identification and login.</value>
    /// <remarks>
    /// Maximum length is 100 chars, more than this will make an overflow.
    /// </remarks>
    public string UserName { get; set; }
    
    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    /// <value>The email address used for communication and account recovery.</value>
    /// <remarks>
    /// Maximum length is 100 chars, more than this will make an overflow.
    /// </remarks>
    public string Email { get; set; }
    
    /// <summary>
    /// Gets or sets the phone number of the user.
    /// </summary>
    /// <value>The phone number used for communication and account verification.</value>
    /// <remarks>
    /// Maximum length is 100 chars, more than this will make an overflow.
    /// </remarks>
    public string PhoneNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the password for the user account.
    /// </summary>
    /// <value>The hashed password for user authentication.</value>
    /// <remarks>
    /// Maximum length is 100 chars, more than this will make an overflow.
    /// This should store a securely hashed version of the password, not plain text.
    /// </remarks>
    public string Password { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of budgets associated with this user.
    /// </summary>
    /// <value>A collection of Budget entities owned by this user.</value>
    /// <remarks>
    /// Navigation property that acts as one-to-many relationship in the database,
    /// facilitates the operations, not loaded for all times.
    /// </remarks>
    public ICollection<Budget> Budgets { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of expenses associated with this user.
    /// </summary>
    /// <value>A collection of Expense entities owned by this user.</value>
    /// <remarks>
    /// Navigation property that acts as one-to-many relationship in the database,
    /// facilitates the operations, not loaded for all times.
    /// </remarks>
    public ICollection<Expense> Expenses { get; set; }
    
    /// <summary>
    /// Gets or sets the collection of incomes associated with this user.
    /// </summary>
    /// <value>A collection of Income entities owned by this user.</value>
    /// <remarks>
    /// Navigation property that acts as one-to-many relationship in the database,
    /// facilitates the operations, not loaded for all times.
    /// </remarks>
    public ICollection<Income> Incomes { get; set; }
    
    /// <summary>
    /// Returns a string representation of the User object.
    /// </summary>
    /// <returns>A string containing the User's properties and their values.</returns>
    public override string ToString()
    {
        return $"User Id: {Id}, Username: {UserName}, Email: {Email}, Phone: {PhoneNumber}, ";
    }
}