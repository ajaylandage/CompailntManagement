using Microsoft.Extensions.DependencyInjection;
using ComplaintManagementSystem.API.Data;
using ComplaintManagementSystem.API.Repositories;
using ComplaintManagementSystem.API.Services;

namespace ComplaintManagementSystem.API.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<IDbConnectionFactory, DbConnectionFactory>();

            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IComplaintRepository, ComplaintRepository>();
            services.AddScoped<ICategoryRepository, CategoryRepository>();

            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IComplaintService, ComplaintService>();
            services.AddScoped<ICategoryService, CategoryService>();
            services.AddScoped<IDashboardService, DashboardService>();

            return services;
        }
    }
}