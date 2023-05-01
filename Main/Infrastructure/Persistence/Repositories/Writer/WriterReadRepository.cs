using Application.Repositories;
using Domain.Entities;
using Persistence.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Persistence.Repositories
{
    public class WriterReadRepository : ReadRepository<Writer>, IWriterReadRepository
    {
        public WriterReadRepository(AppDbContext context) : base(context)
        {
        }
    }
}
