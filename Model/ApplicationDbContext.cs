using Microsoft.EntityFrameworkCore;
using Model.Entities;

namespace Model;

/// <summary>
/// Represents the database context for the Personal Budgeting application.
/// </summary>
/// <remarks>
/// This class serves as the primary point of interaction with the underlying SQL Server database.
/// It manages the connection to the database and provides access to entity sets for all application entities.
/// </remarks>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// The connection string for the SQL Server database.
    /// </summary>
    /// <remarks>
    /// Points to a local SQL Server instance with the "PersonalBudgeting" database.
    /// Integrated security is enabled and server certificate is trusted.
    /// </remarks>
    private readonly string _connectionString = "Server=.;Database=PersonalBudgeting;Integrated Security=True;TrustServerCertificate=True;";

    /// <summary>
    /// Configures the database connection to be used for this context.
    /// </summary>
    /// <param name="optionsBuilder">A builder used to create or modify options for this context.</param>
    /// <remarks>
    /// This method is called for each instance of the context that is created.
    /// The SQL Server provider is configured with the connection string defined in this class.
    /// </remarks>
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);
    }

    /// <summary>
    /// Gets or sets the DbSet of User entities.
    /// </summary>
    /// <value>A DbSet that can be used to query and save instances of User.</value>
    /// <remarks>
    /// Represents the "User" table in the database.
    /// This property enables CRUD operations on User entities.
    /// </remarks>
    public DbSet<User> Users { get; set; }
    
    /// <summary>
    /// Gets or sets the DbSet of Budget entities.
    /// </summary>
    /// <value>A DbSet that can be used to query and save instances of Budget.</value>
    /// <remarks>
    /// Represents the "Budget" table in the database.
    /// This property enables CRUD operations on Budget entities.
    /// </remarks>
    public DbSet<Budget> Budgets { get; set; }
    
    /// <summary>
    /// Gets or sets the DbSet of Expense entities.
    /// </summary>
    /// <value>A DbSet that can be used to query and save instances of Expense.</value>
    /// <remarks>
    /// Represents the "Expense" table in the database.
    /// This property enables CRUD operations on Expense entities.
    /// </remarks>
    public DbSet<Expense> Expenses { get; set; }
    
    /// <summary>
    /// Gets or sets the DbSet of Income entities.
    /// </summary>
    /// <value>A DbSet that can be used to query and save instances of Income.</value>
    /// <remarks>
    /// Represents the "Income" table in the database.
    /// This property enables CRUD operations on Income entities.
    /// </remarks>
    public DbSet<Income> Incomes { get; set; }
    
    // You might want to add this method if you have any model configurations
    /*
    /// <summary>
    /// Configures the model that was discovered by convention from the entity types.
    /// </summary>
    /// <param name="modelBuilder">The builder being used to construct the model for this context.</param>
    /// <remarks>
    /// Use this method to configure fluent API configurations, relationships,
    /// constraints, and other model-specific settings.
    /// </remarks>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Add your model configurations here
    }
    */
}