using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities;

[Table("Income")]
public class Income
{
    [Key]
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    public decimal Amount { get; set; }

    public string IncomeSourceName { get; set; }
    
    public DateTime IncomeDate { get; set; }
    
    
    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
    
    public override string ToString()
    {
        return $"Income(Id={Id}, UserId={UserId}, Amount={Amount:C}, Source='{IncomeSourceName}', Date={IncomeDate:yyyy-MM-dd})";
    }

}