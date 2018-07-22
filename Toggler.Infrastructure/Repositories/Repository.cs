using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Toggler.Domain.SeedWork.Interfaces;

namespace Toggler.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly TogglerContext _context;

        public Repository(TogglerContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<T> CreateAsync(T connectionInfo)
        {
            _context.Set<T>().Add(connectionInfo);

            await _context.SaveChangesAsync();

            return connectionInfo;
        }

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

        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }

        public async Task<T> GetAsync(string name)
        {
            return await _context.Set<T>().FindAsync(name);
        }

        public async Task<T> UpdateAsync(string originalName, T connectionInfo)
        {
            var oldConnectionInfo = await _context.Set<T>().FindAsync(originalName);

            if (oldConnectionInfo != null)
            {
                _context.Set<T>().Remove(oldConnectionInfo);
                _context.Attach(connectionInfo).State = EntityState.Modified;
            }
            else
            {
                _context.Set<T>().Add(connectionInfo);
            }

            await _context.SaveChangesAsync();

            return connectionInfo;
        }
    }
}
