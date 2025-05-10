using Microsoft.EntityFrameworkCore;
using Model.Interfaces;
using Model.Utilities;

namespace Model.Repositories;

public class GenericRepository<TObject> : IRepository<TObject> where TObject : class
{
    protected readonly DbContextFactory _dbContextFactory;

    public GenericRepository()
    {
        _dbContextFactory = new DbContextFactory();
    }
    
    
    public async Task<int> Add(TObject obj)
    {
        await using var _dbContext = _dbContextFactory.CreateDbContext();
        await _dbContext.Set<TObject>().AddAsync(obj);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> Update(TObject obj)
    {
        await using var _dbContext = _dbContextFactory.CreateDbContext();
        
        var keyProperty = _dbContext.Model.FindEntityType(typeof(TObject))!
            .FindPrimaryKey()!
            .Properties
            .First();
        
        var keyValue = typeof(TObject).GetProperty(keyProperty.Name)?.GetValue(obj);

        var actualEntity = await _dbContext.Set<TObject>().FindAsync(keyValue);
        
        foreach (var prop in typeof(TObject).GetProperties())
        {
            var newValue = prop.GetValue(obj);
            if (newValue != null && prop.CanWrite)
            {
                prop.SetValue(actualEntity, newValue);
            }
        }
        
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> Delete(TObject obj)
    {
        await using var _dbContext = _dbContextFactory.CreateDbContext();
        _dbContext.Set<TObject>().Remove(obj);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<TObject?> GetById(int id)
    {
        await using var _dbContext = _dbContextFactory.CreateDbContext();
        var result = await _dbContext.Set<TObject>().FindAsync(id);
        return result;
    }

    public async Task<IEnumerable<TObject>>? GetAll()
    {
        await using var _dbContext = _dbContextFactory.CreateDbContext();
        return await _dbContext.Set<TObject>().ToListAsync();
    }

    public async Task<bool> CheckExist(int id)
    {
        var result = await this.GetById(id);
        return result != null;
    }
}