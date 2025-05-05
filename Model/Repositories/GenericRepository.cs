using Model.Interfaces;

namespace Model.Repositories;

public class GenericRepository<TObject> : IRepository<TObject> where TObject : class 
{
    public Task<int> Add(TObject obj)
    {
        throw new NotImplementedException();
    }

    public Task<int> Update(TObject obj)
    {
        throw new NotImplementedException();
    }

    public Task<int> Delete(TObject obj)
    {
        throw new NotImplementedException();
    }

    public Task<TObject>? GetById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<IEnumerable<TObject>>? GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<bool> CheckExist(int id)
    {
        throw new NotImplementedException();
    }
}