using Application.Repositories;
using Domain.Entities;
using Persistence.Context;

namespace Persistence.Repositories
{
    public class ExpenditureReadRepository : ReadRepository<Expenditure>, IExpenditureReadRepository
    {
        public ExpenditureReadRepository(AppDbContext context) : base(context)
        {
        }
    }
}
