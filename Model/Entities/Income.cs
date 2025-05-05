using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Model.Entities;

public class Income
{
    [Key]
    public int Id { get; set; }
    
    public int UserId { get; set; }
    
    public decimal Amount { get; set; }
    
    public DateTime IncomeDate { get; set; }
    
    
    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
}