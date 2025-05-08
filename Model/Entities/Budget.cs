using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities;

[Table("Budget")]
public class Budget
{
    [Key]
    public int Id { get; set; }
   
    public int UserId { get; set; } // must not be empty
    
    public string BudgetName { get; set; } // maximum 100 chars
    
    public decimal TotalAmountRequired { get; set; } // 10 digits
    
    
    [ForeignKey(nameof(UserId))]
    public User ? User { get; set; }
    
    public override string ToString()
    {
        return $"Budget [Id={Id}, UserId={UserId}, BudgetName={BudgetName}, TotalAmountRequired={TotalAmountRequired}]";
    }

}