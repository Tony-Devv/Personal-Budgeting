using Microsoft.EntityFrameworkCore;
using Model.Interfaces;
using Model.Utilities;

namespace Model.Repositories;

public class GenericRepository<TObject> : IRepository<TObject> where TObject : class
{
    protected readonly ApplicationDbContext _dbContext;

    public GenericRepository()
    {
        _dbContext = new ApplicationDbContext();
    }
    
    
    public async Task<int> Add(TObject obj)
    {
        await _dbContext.Set<TObject>().AddAsync(obj);
        
        Console.WriteLine(typeof(TObject).Name);
        
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> Update(TObject obj)
    {
        _dbContext.Set<TObject>().Update(obj);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<int> Delete(TObject obj)
    {
        _dbContext.Set<TObject>().Remove(obj);
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<TObject?> GetById(int id)
    {
        var result = await _dbContext.Set<TObject>().FindAsync(id);
        return result;
    }

    public async Task<IEnumerable<TObject>>? GetAll()
    {
        return await _dbContext.Set<TObject>().ToListAsync();
    }

    public async Task<bool> CheckExist(int id)
    {
        var result = await this.GetById(id);
        return result != null;
    }
}