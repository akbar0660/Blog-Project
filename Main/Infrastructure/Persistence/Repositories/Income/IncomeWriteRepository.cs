using Application.Repositories;
using Domain.Entities;
using Persistence.Context;
namespace Persistence.Repositories
{
    public class IncomeWriteRepository : WriteRepository<Income>, IIncomeWriteRepository
    {
        public IncomeWriteRepository(AppDbContext context) : base(context)
        {
        }
    }
}
