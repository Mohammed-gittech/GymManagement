
using System.Reflection;
using FluentValidation;
using FluentValidation.AspNetCore;
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

            // Register SubscriptionPlan
            services.AddScoped<ISubscriptionPlanService, SubscriptionPlanService>();

            // Register MemberService
            services.AddScoped<IMemberService, MemberService>();

            // Register validators from assembly
            services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
            services.AddFluentValidationAutoValidation();
            return services;
        }
    }
}