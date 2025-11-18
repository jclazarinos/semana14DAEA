using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Lab10_Lazarinos.Infrastructure.Persistence.Context;
using Lab10_Lazarinos.Application.Interfaces.Persistence;
using Lab10_Lazarinos.Application.Interfaces.Services;
using Lab10_Lazarinos.Infrastructure.Persistence;
using Lab10_Lazarinos.Infrastructure.Services;

namespace Lab10_Lazarinos.Infrastructure.Configuration;

public static class InfrastructureServicesExtensions
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Database Connection
        services.AddDbContext<TicketeraDbContext>(options =>
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            options.UseNpgsql(connectionString);
        });

        // Unit of Work
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<ITicketService, TicketService>();
        services.AddScoped<IJwtService, JwtService>();
        services.AddScoped<IUserService, UserService>(); 
        services.AddScoped<IResponseService, ResponseService>();

        return services;
    }
}