using System.Collections.Generic;
using System.Threading.Tasks;

namespace Toggler.Domain.SeedWork.Interfaces
{
    /// <summary>
    /// Repository interface
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IRepository<T> where T : class
    {
        /// <summary>
        /// Gets all the entity records asynchronously.
        /// </summary>
        /// <returns>Returns the list of records asynchronously.</returns>
        Task<IEnumerable<T>> GetAllAsync();

        /// <summary>
        /// Gets the specific entity asynchronously by their unique identifier.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Return the entity asynchronously.</returns>
        Task<T> GetAsync(string name);

        /// <summary>
        /// Creates the entity asynchronously.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Return the created entity asynchronously.</returns>
        Task<T> CreateAsync(T entity);

        /// <summary>
        /// Updates the entity asynchronously.
        /// </summary>
        /// <param name="originalName">Unique name of the entity.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>Return the updated entity asynchronously.</returns>
        Task<T> UpdateAsync(string originalName, T entity);

        /// <summary>
        /// Deletes the entity asynchronously.
        /// </summary>
        /// <param name="name">The unique name of the entity.</param>
        /// <returns>Return true/false asynchronously depending upon the success/failure.</returns>
        Task<bool> DeleteAsync(string name);
    }
}
