using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities;

[Table("User")]
public class User
{
    [Key]
    public int Id { get; set; }
    
    public string UserName { get; set; }
    
    public string Email { get; set; }
    
    public string PhoneNumber { get; set; }
    
    public string Password { get; set; }
    
    public ICollection<Budget> Budgets { get; set; }
    public ICollection<Expense> Expenses { get; set; }
    public ICollection<Income> Incomes { get; set; }
    
    
    public override string ToString()
    {
        return $"User Id: {Id}, Username: {UserName}, Email: {Email}, Phone: {PhoneNumber}, ";
    }
}