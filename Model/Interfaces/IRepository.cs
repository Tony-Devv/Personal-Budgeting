namespace Model.Interfaces;

public interface IRepository<TObject> where TObject : class
{
    Task<int> Add(TObject obj);

    Task<int> Update(TObject obj);

    Task<int> Delete(TObject obj);

    Task<TObject?>? GetById(int id);

    Task<IEnumerable<TObject>>?  GetAll();

    Task<bool> CheckExist(int id);
}