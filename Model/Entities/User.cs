namespace Model.Entities;

public class User
{
    public int Id { get; set; }
    
    public string UserName { get; set; }
    
    public string Email { get; set; }
    
    public string PhoneNumber { get; set; }
    
    public string Password { get; set; }
    
    public ICollection<Budget> Budgets { get; set; }
    public ICollection<Expense> Expenses { get; set; }
    public ICollection<Income> Incomes { get; set; }
}