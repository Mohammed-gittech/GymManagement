using GymManagement.DAL.Data;
using GymManagement.DAL.Repositories;
using GymManagement.DAL.Repositories.Interfaces;
using GymManagement.DAL.Services;
using DALUnitOfWork = GymManagement.DAL.UnitOfWork;
using GymManagement.DAL.UnitOfWork;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GymManagement.DAL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDAL(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Register AppDbContext with SQL Server
            services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            // Register repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

            // Register UnitOfWork
            services.AddScoped<IUnitOfWork, DALUnitOfWork.UnitOfWork>();

            // Register SubscriptionPlan
            services.AddScoped<ISubscriptionPlanRepository, SubscriptionPlanRepository>();

            // Register MemberRepository
            services.AddScoped<IMemberRepository, MemberRepository>();

            // Register SubscriptionResponse
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();

            // Register Payment
            services.AddScoped<IPaymentRepository, PaymentRepository>();

            // Register CurrentUserService
            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();
            return services;
        }
    }
}