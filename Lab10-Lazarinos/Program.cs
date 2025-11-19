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

// Intentar múltiples fuentes
var connectionString = 
    builder.Configuration.GetConnectionString("DefaultConnection") 
    ?? builder.Configuration["DATABASE_URL"]
    ?? Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

// DEBUG: Log detallado para identificar el problema
var logFile = "/tmp/startup-debug.log";
try 
{
    var debugInfo = $@"
=== RENDER STARTUP DEBUG ===
Timestamp: {DateTime.UtcNow}
ConnectionString found: {!string.IsNullOrEmpty(connectionString)}
ConnectionString length: {connectionString?.Length ?? 0}
ConnectionString (first 50 chars): {connectionString?.Substring(0, Math.Min(50, connectionString?.Length ?? 0))}

Environment Variables:
- DATABASE_URL: {Environment.GetEnvironmentVariable("DATABASE_URL")?.Substring(0, Math.Min(50, Environment.GetEnvironmentVariable("DATABASE_URL")?.Length ?? 0)) ?? "NOT SET"}
- ConnectionStrings__DefaultConnection: {Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")?.Substring(0, Math.Min(50, Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")?.Length ?? 0)) ?? "NOT SET"}

Configuration:
- GetConnectionString('DefaultConnection'): {builder.Configuration.GetConnectionString("DefaultConnection")?.Substring(0, Math.Min(50, builder.Configuration.GetConnectionString("DefaultConnection")?.Length ?? 0)) ?? "NOT SET"}
- DATABASE_URL from config: {builder.Configuration["DATABASE_URL"]?.Substring(0, Math.Min(50, builder.Configuration["DATABASE_URL"]?.Length ?? 0)) ?? "NOT SET"}
===========================
";
    
    Console.WriteLine(debugInfo);
    File.WriteAllText(logFile, debugInfo);
}
catch (Exception ex)
{
    Console.WriteLine($"Debug logging failed: {ex.Message}");
}

if (string.IsNullOrWhiteSpace(connectionString))
{
    var errorMsg = @"
❌ ERROR: Database connection string not found!
Please configure the DATABASE_URL environment variable in Render.

Steps to fix:
1. Go to your Web Service in Render Dashboard
2. Click 'Environment' in the sidebar
3. Add environment variable:
   - Key: DATABASE_URL
   - Value: Your PostgreSQL Internal Connection String
4. Save and redeploy
";
    Console.WriteLine(errorMsg);
    throw new InvalidOperationException(errorMsg);
}

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