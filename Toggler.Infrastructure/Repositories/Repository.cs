using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Toggler.Domain.SeedWork.Interfaces;

namespace Toggler.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for entity
    /// </summary>
    /// <typeparam name="T">Entity</typeparam>
    /// <seealso cref="Toggler.Domain.SeedWork.Interfaces.IRepository{T}" />
    public class Repository<T> : IRepository<T> where T : class
    {
        /// <summary>
        /// The toggler context
        /// </summary>
        private readonly TogglerContext _context;

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <exception cref="ArgumentNullException">context</exception>
        public Repository(TogglerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Creates the entity asynchronously.
        /// </summary>
        /// <param name="entity">The entity.</param>
        /// <returns>Return the created entity asynchronously.</returns>
        public async Task<T> CreateAsync(T entity)
        {
            _context.Set<T>().Add(entity);

            await _context.SaveChangesAsync();

            return entity;
        }

        /// <summary>
        /// Deletes the entity asynchronously.
        /// </summary>
        /// <param name="name">The unique name of the entity.</param>
        /// <returns>Return true/false asynchronously depending upon the success/failure.</returns>
        public async Task<bool> DeleteAsync(string name)
        {
            var connectionInfo = await _context.Set<T>().FindAsync(name);
            if (connectionInfo == null)
            {
                return false;
            }
            _context.Set<T>().Remove(connectionInfo);
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// Gets all the entity records asynchronously.
        /// </summary>
        /// <returns>Returns the list of records asynchronously.</returns>
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        /// <summary>
        /// Gets the specific entity asynchronously by their unique identifier.
        /// </summary>
        /// <param name="name">The name.</param>
        /// <returns>Return the entity asynchronously.</returns>
        public async Task<T> GetAsync(string name)
        {
            return await _context.Set<T>().FindAsync(name);
        }

        /// <summary>
        /// Updates the entity asynchronously.
        /// </summary>
        /// <param name="originalName">Unique name of the entity.</param>
        /// <param name="entity">The entity.</param>
        /// <returns>Return the updated entity asynchronously.</returns>
        public async Task<T> UpdateAsync(string originalName, T entity)
        {
            var oldConnectionInfo = await _context.Set<T>().FindAsync(originalName);

            if (oldConnectionInfo != null)
            {
                _context.Set<T>().Remove(oldConnectionInfo);
                _context.Attach(entity).State = EntityState.Modified;
            }
            else
            {
                _context.Set<T>().Add(entity);
            }

            await _context.SaveChangesAsync();

            return entity;
        }
    }
}
