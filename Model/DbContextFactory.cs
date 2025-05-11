using Microsoft.EntityFrameworkCore;

namespace Model;


/// <summary>
///  Factory Pattern to Create ApplicationDbContext Variable
/// </summary>
/// <remarks>
/// Work as a way to facilitate the async operations since the dbcotnext itself is not thread safe
/// its being used in the repositories, almost any operation will create a DbContext to ensure that its thread safe
/// </remarks>
public class DbContextFactory
{

    /// <returns> returns a specific DbContext which is the ApplicationDbContext</returns>
    public ApplicationDbContext CreateDbContext()
    {
        return new ApplicationDbContext();
    }
    
}