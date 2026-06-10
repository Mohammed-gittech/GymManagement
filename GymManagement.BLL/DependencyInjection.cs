
using System.Reflection;
using FluentValidation;
using GymManagement.BLL.Services;
using GymManagement.BLL.Services.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace GymManagement.BLL
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBLL(
            this IServiceCollection services
        )
        {
            // Register AuthService
            services.AddScoped<IAuthService, AuthService>();

            // Register validators from assembly
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            return services;
        }
    }
}