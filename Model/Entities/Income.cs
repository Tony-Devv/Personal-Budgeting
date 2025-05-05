namespace Model.Entities;

public class Income
{
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    public decimal Amount { get; set; }
    
    public DateTime IncomeDate { get; set; }
}