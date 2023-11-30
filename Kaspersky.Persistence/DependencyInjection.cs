using Kaspersky.Application.Persistence.Contracts;
using Kaspersky.Domain.Common.Contracts;
using Kaspersky.Persistence.Contracts;
using Kaspersky.Persistence.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.Net.Mail;
using System.Text;

namespace Kaspersky.Persistence
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApiDbContext>(item => item.UseSqlServer(configuration.GetConnectionString("DefaultConnect")));
            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApiDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.SaveToken = true;
                options.RequireHttpsMetadata = false;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidAudience = configuration["JWT:ValidAudience"],
                    ValidIssuer = configuration["JWT:ValidIssuer"],
                    IssuerSigningKey = new SymmetricSecurityKey
                    (Encoding.UTF8.GetBytes(configuration["JWT:Secret"] ?? ""))
                };
            })
            .AddCookie(options =>
            {
                // Cấu hình Cookie Authentication
                options.ExpireTimeSpan = TimeSpan.FromDays(2); // Thời gian sống của Cookie
                options.SlidingExpiration = true; // Tự động gia hạn thời gian sống khi có hoạt động
            });

            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.Scan(scan => scan
                .FromAssemblyOf<GenericRepository<object>>()
                .AddClasses(classes => classes
                    .Where(type => type.IsClass
                        && !type.IsAbstract
                        && type.GetInterfaces().Any(i =>
                            i.IsGenericType
                            && i.GetGenericTypeDefinition() == typeof(IGenericRepository<>))))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new RequireHttpsAttribute());
            });
            services.AddTransient<SmtpClient>();
            services.AddTransient<HttpClient>();
            return services;
        }
    }
}
