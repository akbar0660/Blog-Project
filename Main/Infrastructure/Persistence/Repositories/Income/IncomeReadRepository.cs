using Application.Repositories;
using Domain.Entities;
using Persistence.Context;

namespace Persistence.Repositories
{
    public class IncomeReadRepository : ReadRepository<Income>, IIncomeReadRepository
    {
        public IncomeReadRepository(AppDbContext context) : base(context)
        {
        }
    }
}
