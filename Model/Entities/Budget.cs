namespace Model.Entities;

public class Budget
{
    public int Id { get; set; }
   
    public int UserId { get; set; }
    
    public string BudgetName { get; set; }
    
    public decimal TotalAmountRequired { get; set; }
}