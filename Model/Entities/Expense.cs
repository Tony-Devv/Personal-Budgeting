namespace Model.Entities;

public class Expense
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    public int BudgetId { get; set; }
    
    public string ExpenseName { get; set; }
    
    public decimal RequiredAmount { get; set; }
    
    public DateTime DateCycle { get; set; }
    
    public decimal SpentAmount { get; set; }
    
    public DateTime ? ReminderTime { get; set; }
}