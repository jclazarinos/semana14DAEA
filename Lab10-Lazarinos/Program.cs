using Hangfire;
using Hangfire.PostgreSql;
using Lab10_Lazarinos.Application.Interfaces.Services;
using Lab10_Lazarinos.Configuration;
using Lab10_Lazarinos.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

// ========== AGREGADO PARA RENDER ==========
// Configurar para escuchar en el puerto de Render
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
// ==========================================

// Agregar servicios personalizados
builder.Services.AddApplicationServices(builder.Configuration);

// Add services to the container.
builder.Services.AddOpenApi();

// ========== MODIFICADO PARA RENDER ==========
// Configurar Hangfire con PostgreSQL Storage
// Ahora lee desde variables de entorno o appsettings
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    ?? throw new InvalidOperationException("Database connection string not found");

builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(connectionString));
// ============================================

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

// ========== AGREGADO PARA RENDER ==========
// También habilitar Swagger en producción para Render
if (app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Ticketera API v1");
        c.RoutePrefix = "swagger"; // Swagger en /swagger en producción
    });
}
// ==========================================

// ========== COMENTAR ESTA LÍNEA PARA RENDER ==========
// Render maneja HTTPS automáticamente, esto causará problemas
// app.UseHttpsRedirection();
// ======================================================

// Middleware para dashboard de Hangfire
app.UseHangfireDashboard("/hangfire");

// Programar limpieza de datos
RecurringJob.AddOrUpdate(
    "db-cleanup-job", // ID único
    () => new DataCleanupService().PerformDatabaseCleanup(), 
    "0 2 * * *"); // Expresión Cron: Todos los días a las 2:00 AM

// ========== AGREGADO PARA RENDER ==========
// Health check endpoint para Render
app.MapGet("/health", () => Results.Ok(new 
{ 
    status = "healthy", 
    timestamp = DateTime.UtcNow,
    database = !string.IsNullOrEmpty(connectionString) ? "configured" : "not configured"
}));
// ==========================================

// IMPORTANTE: El orden importa
app.UseAuthentication(); // Primero autenticación
app.UseAuthorization();  // Luego autorización

app.MapControllers();

app.Run();