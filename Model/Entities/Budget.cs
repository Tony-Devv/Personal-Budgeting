using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities;

/// <summary>
/// Represents a budget entity in the system.
/// it's being used by the entityFramework to represent the budgets records
/// </summary>
/// <remarks>
/// This class maps to the "Budget" table in the database and maintains
/// relationship with the User entity through a foreign key.
/// </remarks>
[Table("Budget")]
public class Budget
{
    /// <summary>
    /// Gets or sets the unique identifier for the budget.
    /// </summary>
    /// <value>An integer that uniquely identifies the budget.</value>
    /// <remarks>
    ///  Can't Be 0 or Negative if your performing update,delete,read operations
    /// </remarks>
    [Key]
    public int Id { get; set; }
   
    /// <summary>
    /// Gets or sets the identifier of the user who owns this budget.
    /// </summary>
    /// <value>The user ID. This field is required and cannot be empty.</value>
    /// <remarks>
    ///  Can't Be 0 or Negative if your performing update,delete,read operations
    /// </remarks>
    public int UserId { get; set; } // must not be empty
    
    /// <summary>
    /// Gets or sets the name of the budget.
    /// </summary>
    /// <value>The budget name. Maximum length is 100 characters.</value>
    /// <remarks>
    ///  Maximum Length is 100 chars, more than this will make an overflow
    /// </remarks>
    public string BudgetName { get; set; } // maximum 100 chars
    
    /// <summary>
    /// Gets or sets the total amount required for this budget.
    /// </summary>
    /// <value>The total amount required. Can have up to 10 digits.</value>
    /// <remarks>
    ///  Maximum digits allowed is 6
    /// </remarks>
    public decimal TotalAmountRequired { get; set; }
    
    /// <summary>
    /// Gets or sets the associated user entity.
    /// </summary>
    /// <value>The user who owns this budget. Can be null.</value>
    /// <remarks>
    ///     Navigation Property that acts as many-to-one relationship in the database,
    ///     facilitate the operations, not loaded for all times  
    /// </remarks>
    [ForeignKey(nameof(UserId))]
    public User? User { get; set; }
    
    /// <summary>
    /// Returns a string representation of the Budget object.
    /// </summary>
    /// <returns>A string containing the Budget's properties and their values.</returns>
    public override string ToString()
    {
        return $"Budget [Id={Id}, UserId={UserId}, BudgetName={BudgetName}, TotalAmountRequired={TotalAmountRequired}]";
    }
}