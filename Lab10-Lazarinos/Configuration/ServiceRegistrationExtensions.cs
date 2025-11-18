using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using System.Reflection;
using Lab10_Lazarinos.Infrastructure.Configuration;
using MediatR;

namespace Lab10_Lazarinos.Configuration;

public static class ServiceRegistrationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Registra el servicio IHttpContextAccessor
        services.AddHttpContextAccessor();
        
        // Registrar MediatR
        services.AddMediatR(cfg => {
            cfg.RegisterServicesFromAssembly(typeof(Lab10_Lazarinos.Application.AssemblyReference).Assembly);
        });
        
        // Registra de servicios de Infraestructura
        services.AddInfrastructureServices(configuration);
        
        // Habilitar la autenticacion
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["Jwt:Issuer"],
                    ValidAudience = configuration["Jwt:Audience"],
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(configuration["Jwt:SecretKey"] 
                            ?? throw new InvalidOperationException("Jwt:SecretKey configuration is missing")))
                };
            });

        // Habilitar controladores
        services.AddControllers();
        
        // Habilitar Swagger
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            // Información de la API
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Ticketera API - Sistema de Gestión de Tickets",
                Version = "v1",
                Description = "API RESTful para sistema de helpdesk con autenticación JWT",
                Contact = new OpenApiContact
                {
                    Name = "Soporte Técnico",
                    Email = "soporte@ticketera.com"
                }
            });

            // ✅ CONFIGURACIÓN DE SEGURIDAD JWT
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Ingresa tu token JWT en el formato: Bearer {token}\n\n" +
                              "Ejemplo: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9..."
            });

            // ✅ REQUERIMIENTO DE SEGURIDAD GLOBAL
            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });

            // Incluir comentarios XML si existen
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            if (File.Exists(xmlPath))
            {
                options.IncludeXmlComments(xmlPath);
            }
        });

        return services;
    }
}