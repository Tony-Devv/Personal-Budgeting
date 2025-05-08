using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities;


[Table("Expense")]
public class Expense
{
    [Key]
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    public int BudgetId { get; set; }
    
    public string ExpenseName { get; set; } // length of 100 maximum
    
    public decimal RequiredAmount { get; set; }
    
    public DateTime DateCycle { get; set; }
    
    public decimal SpentAmount { get; set; }
    
    public DateTime ? ReminderTime { get; set; }
    
    [ForeignKey(nameof(UserId))]
    public User  User { get; set; }
    
    [ForeignKey(nameof(BudgetId))]
    public Budget Budget { get; set; }
    
    public override string ToString()
    {
        return $"Expense [Id={Id}, UserId={UserId}, BudgetId={BudgetId}, ExpenseName={ExpenseName}, RequiredAmount={RequiredAmount}, SpentAmount={SpentAmount}, DateCycle={DateCycle}, ReminderTime={ReminderTime}]";
    }
}