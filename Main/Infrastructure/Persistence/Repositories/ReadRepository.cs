using Application.Repositories;
using Domain.Entities.Common;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class ReadRepository<T> : IReadRepository<T> where T : BaseEntity
    {
        private readonly AppDbContext _context;
        public ReadRepository(AppDbContext context)
        {
            _context=context;
        }
        public DbSet<T> Table => _context.Set<T>();

        public IQueryable<T> GetAll()
        => Table;

        public IQueryable<T> GetWhere(Expression<Func<T, bool>> method)
        =>Table.Where(method);

        public async Task<T> GetSingleAsync(Expression<Func<T, bool>> method)
        =>await Table.FirstOrDefaultAsync(method);

        public async Task<T> GetByIdAsync(int? id)
        => await Table.FirstOrDefaultAsync(data=>data.Id==id);

        public Task<bool> IsExist(int id)
        {
            return Table.AnyAsync(x => x.Id == id);
        }

        public async Task<int> CountAsync()
        {
             return await Table.CountAsync();
        }
    }
}
