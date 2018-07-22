using System.Collections.Generic;
using System.Threading.Tasks;

namespace Toggler.Domain.SeedWork.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetAsync(string name);
        Task<T> CreateAsync(T connectionInfo);
        Task<T> UpdateAsync(string originalName, T connectionInfo);
        Task<bool> DeleteAsync(string name);
    }
}
