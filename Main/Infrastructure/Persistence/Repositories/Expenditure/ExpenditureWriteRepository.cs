using Application.Repositories;
using Domain.Entities;
using Persistence.Context;

namespace Persistence.Repositories
{
    public class ExpenditureWriteRepository : WriteRepository<Expenditure>, IExpenditureWriteRepository
    {
        public ExpenditureWriteRepository(AppDbContext context) : base(context)
        {
        }
    }
}
