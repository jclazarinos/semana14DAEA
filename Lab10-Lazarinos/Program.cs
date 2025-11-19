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
// IMPORTANTE: En producción (Render), usar SOLO variables de entorno

string? connectionString = null;

// En producción, SOLO usar variables de entorno
if (builder.Environment.IsProduction())
{
    // Render usa DATABASE_URL en formato postgresql://
    var databaseUrl = Environment.GetEnvironmentVariable("DATABASE_URL")
        ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");
    
    if (!string.IsNullOrEmpty(databaseUrl))
    {
        // Convertir de postgresql:// a formato Npgsql
        connectionString = ConvertRenderConnectionString(databaseUrl);
    }
    
    Console.WriteLine("🌐 PRODUCTION MODE: Using environment variables only");
}
else
{
    // En desarrollo, usar appsettings.json
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
    Console.WriteLine("💻 DEVELOPMENT MODE: Using appsettings.json");
}

// DEBUG: Log detallado
try 
{
    var debugInfo = $@"
=== RENDER STARTUP DEBUG ===
Timestamp: {DateTime.UtcNow}
Environment: {builder.Environment.EnvironmentName}
ConnectionString found: {!string.IsNullOrEmpty(connectionString)}
ConnectionString CONVERTED: {connectionString ?? "NULL"}
===========================
";
    
    Console.WriteLine(debugInfo);
}
catch (Exception ex)
{
    Console.WriteLine($"Debug logging failed: {ex.Message}");
}

if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException("Database connection string not found!");
}

builder.Services.AddHangfire(config =>
    config.UsePostgreSqlStorage(connectionString));

// Función helper para convertir connection string de Render
static string ConvertRenderConnectionString(string databaseUrl)
{
    try
    {
        var uri = new Uri(databaseUrl);
        
        var host = uri.Host;
        var port = uri.Port > 0 ? uri.Port : 5432;
        var database = uri.LocalPath.TrimStart('/');
        var username = uri.UserInfo.Split(':')[0];
        var password = uri.UserInfo.Split(':')[1];
        
        return $"Host={host};Port={port};Database={database};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Error converting connection string: {ex.Message}");
        throw;
    }
}
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