using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


/// <summary>
/// Represents an expense entity in the system.
/// It's being used by EntityFramework to represent the expenses records.
/// </summary>
/// <remarks>
/// This class maps to the "Expense" table in the database and maintains
/// relationships with both User and Budget entities through foreign keys.
/// </remarks>
[Table("Expense")]
public class Expense
{
    /// <summary>
    /// Gets or sets the unique identifier for the expense.
    /// </summary>
    /// <value>An integer that uniquely identifies the expense.</value>
    /// <remarks>
    /// Can't be 0 or negative if you're performing update, delete, or read operations.
    /// </remarks>
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the identifier of the user who owns this expense.
    /// </summary>
    /// <value>The user ID. This field is required and cannot be empty.</value>
    /// <remarks>
    /// Can't be 0 or negative if you're performing update, delete, or read operations.
    /// </remarks>
    public int UserId { get; set; }
    
    /// <summary>
    /// Gets or sets the identifier of the budget associated with this expense.
    /// </summary>
    /// <value>The budget ID. This field is required and cannot be empty.</value>
    /// <remarks>
    /// Can't be 0 or negative if you're performing update, delete, or read operations.
    /// </remarks>
    public int BudgetId { get; set; }
    
    /// <summary>
    /// Gets or sets the name of the expense.
    /// </summary>
    /// <value>The expense name.</value>
    /// <remarks>
    /// Maximum length is 100 chars, more than this will make an overflow.
    /// </remarks>
    public string ExpenseName { get; set; } // length of 100 maximum
    
    /// <summary>
    /// Gets or sets the amount required for this expense.
    /// </summary>
    /// <value>The required amount for this expense.</value>
    /// <remarks>
    /// Maximum digits allowed is 6.
    /// </remarks>
    public decimal RequiredAmount { get; set; }
    
    /// <summary>
    /// Gets or sets the date cycle for this expense.
    /// </summary>
    /// <value>The date cycle when this expense occurs.</value>
    public DateTime DateCycle { get; set; }
    
    /// <summary>
    /// Gets or sets the amount that has been spent for this expense.
    /// </summary>
    /// <value>The amount that has been spent.</value>
    /// <remarks>
    /// Maximum digits allowed is 6.
    /// </remarks>
    public decimal SpentAmount { get; set; }
    
    /// <summary>
    /// Gets or sets the reminder time for this expense.
    /// </summary>
    /// <value>The reminder time. Can be null if no reminder is set.</value>
    public DateTime? ReminderTime { get; set; }
    
    /// <summary>
    /// Gets or sets the associated user entity.
    /// </summary>
    /// <value>The user who owns this expense.</value>
    /// <remarks>
    /// Navigation property that acts as many-to-one relationship in the database,
    /// facilitates the operations, not loaded for all times.
    /// </remarks>
    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
    
    /// <summary>
    /// Gets or sets the associated budget entity.
    /// </summary>
    /// <value>The budget associated with this expense.</value>
    /// <remarks>
    /// Navigation property that acts as many-to-one relationship in the database,
    /// facilitates the operations, not loaded for all times.
    /// </remarks>
    [ForeignKey(nameof(BudgetId))]
    public Budget Budget { get; set; }
    
    /// <summary>
    /// Returns a string representation of the Expense object.
    /// </summary>
    /// <returns>A string containing the Expense's properties and their values.</returns>
    public override string ToString()
    {
        return $"Expense [Id={Id}, UserId={UserId}, BudgetId={BudgetId}, ExpenseName={ExpenseName}, RequiredAmount={RequiredAmount}, SpentAmount={SpentAmount}, DateCycle={DateCycle}, ReminderTime={ReminderTime}]";
    }
}