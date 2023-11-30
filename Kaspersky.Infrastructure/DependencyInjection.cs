using Kaspersky.Infrastructure.InterfaceService;
using Kaspersky.Infrastructure.Service;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Kaspersky.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddDIService(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IRefreshTokenService, RefreshTokenService>();
            services.AddScoped<IAccountService, AccountService>();
            return services;
        }
    }
}
