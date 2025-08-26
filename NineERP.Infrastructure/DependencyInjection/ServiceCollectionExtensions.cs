using Microsoft.Extensions.DependencyInjection;
using NineERP.Application.Interfaces.Identity;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Infrastructure.Contexts;
using NineERP.Infrastructure.Services.Common;
using NineERP.Infrastructure.Services.Identity;
using System.Reflection;
using NineERP.Application.Interfaces.Common;

namespace NineERP.Infrastructure.DependencyInjection
{
    public static class ServiceCollectionExtensions
    {
        public static void AddInfrastructureMappings(this IServiceCollection services)
        {
            services.AddAutoMapper(Assembly.GetExecutingAssembly());
        }

        public static void AddApplicationServices(this IServiceCollection services)
        {
            // Binding interface DbContext
            services.AddScoped<IApplicationDbContext, ApplicationDbContext>();
            // System
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IDateTimeService, DateTimeService>();
            services.AddScoped<IDatabaseSeeder, DatabaseSeeder>();

            // Identity
            services.AddScoped<ITokenService, IdentityService>();

            // Entities
        }
    }
}
