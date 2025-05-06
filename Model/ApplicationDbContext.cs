using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace Model;

public class ApplicationDbContext : DbContext
{
    private readonly string _connectionString = "Server=.;Database=PersonalBudgeting;Integrated Security=True;TrustServerCertificate=True;";

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Budget> Budgets{ get; set; }
    public DbSet<Expense> Expenses{ get; set; }
    public DbSet<Income> Incomes{ get; set; }
}