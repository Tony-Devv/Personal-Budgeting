using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


/// <summary>
/// Represents an income entity in the system.
/// It's being used by EntityFramework to represent the income records.
/// </summary>
/// <remarks>
/// This class maps to the "Income" table in the database and maintains
/// relationship with the User entity through a foreign key.
/// </remarks>
[Table("Income")]
public class Income
{
    /// <summary>
    /// Gets or sets the unique identifier for the income.
    /// </summary>
    /// <value>An integer that uniquely identifies the income.</value>
    /// <remarks>
    /// Can't be 0 or negative if you're performing update, delete, or read operations.
    /// </remarks>
    [Key]
    public int Id { get; set; }
    
    /// <summary>
    /// Gets or sets the identifier of the user who owns this income.
    /// </summary>
    /// <value>The user ID. This field is required and cannot be empty.</value>
    /// <remarks>
    /// Can't be 0 or negative if you're performing update, delete, or read operations.
    /// </remarks>
    public int UserId { get; set; }
    
    /// <summary>
    /// Gets or sets the amount of the income.
    /// </summary>
    /// <value>The monetary amount of this income.</value>
    /// <remarks>
    /// Maximum digits allowed is 6.
    /// </remarks>
    public decimal Amount { get; set; }

    /// <summary>
    /// Gets or sets the name of the income source.
    /// </summary>
    /// <value>The name of the source from which this income was received.</value>
    /// <remarks>
    /// Maximum length is 100 chars, more than this will make an overflow.
    /// </remarks>
    public string IncomeSourceName { get; set; }
    
    /// <summary>
    /// Gets or sets the date when this income was received.
    /// </summary>
    /// <value>The date when this income was received or recorded.</value>
    public DateTime IncomeDate { get; set; }
    
    /// <summary>
    /// Gets or sets the associated user entity.
    /// </summary>
    /// <value>The user who owns this income.</value>
    /// <remarks>
    /// Navigation property that acts as many-to-one relationship in the database,
    /// facilitates the operations, not loaded for all times.
    /// </remarks>
    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
    
    /// <summary>
    /// Returns a string representation of the Income object.
    /// </summary>
    /// <returns>A string containing the Income's properties and their values,
    /// with the Amount formatted as currency and Date formatted as yyyy-MM-dd.</returns>
    public override string ToString()
    {
        return $"Income(Id={Id}, UserId={UserId}, Amount={Amount:C}, Source='{IncomeSourceName}', Date={IncomeDate:yyyy-MM-dd})";
    }
}