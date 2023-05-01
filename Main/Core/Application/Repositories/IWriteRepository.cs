using Domain.Entities.Common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Application.Repositories
{
    public interface IWriteRepository<T> : IRepository<T> where T : BaseEntity
    {
        Task<bool> AddAsync(T model);
        Task<bool> AddRangeAsync(List<T> datas);
        bool Remove(T model);
        Task<bool> Remove(int id);
        bool RemoveRange(List<T> datas);    
        void Update(T model);
        Task<int> SaveChangesAsync();

    }
}
