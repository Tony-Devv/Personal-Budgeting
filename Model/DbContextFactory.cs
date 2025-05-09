using Microsoft.EntityFrameworkCore;

namespace Model;

public class DbContextFactory
{

    public ApplicationDbContext CreateDbContext()
    {
        return new ApplicationDbContext();
    }
    
}