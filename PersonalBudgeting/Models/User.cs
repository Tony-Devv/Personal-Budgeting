using System.Collections.Generic;
using Model.Entities;

namespace PersonalBudgeting.Models
{
    public class User
    {
        public int Id { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string Password { get; set; } = string.Empty;
        
        // Navigation properties for related data
        public List<Income> Incomes { get; set; } = new List<Income>();
        public List<Expense> Expenses { get; set; } = new List<Expense>();
        public List<Budget> Budgets { get; set; } = new List<Budget>();
    }
} 