namespace Model.Interfaces
{
    /// <summary>
    /// Defines a generic repository interface that provides basic Create, Read, Update, and Delete (CRUD) operations.
    /// </summary>
    /// <typeparam name="TObject">The type of the entity the repository will manage. Must be a reference type.</typeparam>
    public interface IRepository<TObject> where TObject : class
    {
        /// <summary>
        /// Adds a new object to the underlying data store.
        /// </summary>
        /// <param name="obj">The object to add.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of records affected.</returns>
        Task<int> Add(TObject obj);

        /// <summary>
        /// Updates an existing object in the data store.
        /// </summary>
        /// <param name="obj">The object to update.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of records affected.</returns>
        Task<int> Update(TObject obj);

        /// <summary>
        /// Deletes an object from the data store.
        /// </summary>
        /// <param name="obj">The object to delete.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the number of records affected.</returns>
        Task<int> Delete(TObject obj);

        /// <summary>
        /// Retrieves an object by its unique identifier.
        /// </summary>
        /// <param name="id">The identifier of the object to retrieve.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains the object if found; otherwise, null.</returns>
        Task<TObject?> GetById(int id);

        /// <summary>
        /// Retrieves all objects from the data store.
        /// </summary>
        /// <returns>A task that represents the asynchronous operation. The task result contains a collection of all objects.</returns>
        Task<IEnumerable<TObject>> GetAll();

        /// <summary>
        /// Checks whether an object with the specified ID exists in the data store.
        /// </summary>
        /// <param name="id">The identifier of the object to check for existence.</param>
        /// <returns>A task that represents the asynchronous operation. The task result is true if the object exists; otherwise, false.</returns>
        Task<bool> CheckExist(int id);
    }
}
