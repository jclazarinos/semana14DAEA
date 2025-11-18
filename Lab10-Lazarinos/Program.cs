using Hangfire;
using Hangfire.PostgreSql;
using Lab10_Lazarinos.Application.Interfaces.Services;
using Lab10_Lazarinos.Configuration;
using Lab10_Lazarinos.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// Agregar servicios personalizados
builder.Services.AddApplicationServices(builder.Configuration);

// Add services to the container.
builder.Services.AddOpenApi();

// Configurar Hangfire con SQL Server Storage
builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(builder.Configuration.GetConnectionString(
        "DefaultConnection")));

builder.Services.AddHangfireServer();

builder.Services.AddScoped<IExcelGeneratorService, ExcelGeneratorService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ticketera API v1");
        c.RoutePrefix = string.Empty; // Swagger en la raíz
    });
}

app.UseHttpsRedirection();

// Middleware para dashboard de Hangfire
app.UseHangfireDashboard("/hangfire");

// Programar limpieza de datos
RecurringJob.AddOrUpdate(
    "db-cleanup-job", // ID único
    () => new DataCleanupService().PerformDatabaseCleanup(), 
    "0 2 * * *"); // Expresión Cron: Todos los días a las 2:00 AM


// IMPORTANTE: El orden importa
app.UseAuthentication(); // Primero autenticación
app.UseAuthorization();  // Luego autorización

app.MapControllers();

app.Run();