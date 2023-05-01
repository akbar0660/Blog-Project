using Application.Repositories;
using Application.Repositories.EmailSender;
using Microsoft.Extensions.DependencyInjection;
using Persistence.Repositories;
using Persistence.Repositories.EmailSender;
namespace Persistence
{
    public static class ServiceRegistrations
    {
        public static void AddPersistenceServices(this IServiceCollection services)
        {
            services.AddScoped<ICategoryReadRepository,CategoryReadRepository>();
            services.AddScoped<ICategoryWriteRepository,CategoryWriteRepository>();
            services.AddScoped<IBlogReadRepository,BlogReadRepository>();
            services.AddScoped<IBlogWriteRepository,BlogWriteRepository>();
            services.AddScoped<IWriterWriteRepository,WriterWriteRepository>();
            services.AddScoped<IWriterReadRepository,WriterReadRepository>();
            services.AddScoped<ICommentReadRepository,CommentReadRepository>();
            services.AddScoped<ICommentWriteRepository,CommentWriteRepository>();
            services.AddScoped<IEmailSenderRepository, EmailSenderRepository>();
            services.AddScoped<IIncomeReadRepository, IncomeReadRepository>();
            services.AddScoped<IIncomeWriteRepository, IncomeWriteRepository>();
            services.AddScoped<IExpenditureWriteRepository, ExpenditureWriteRepository>();
            services.AddScoped<IExpenditureReadRepository, ExpenditureReadRepository>();

        }
    }
}
