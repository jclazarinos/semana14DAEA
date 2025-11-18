using Microsoft.AspNetCore.Mvc;
using Hangfire;
using Lab10_Lazarinos.Infrastructure.Services;
using System;

namespace Lab10_Lazarinos.Controllers;

[ApiController]
[Route("api/hangfire-test")]

public class HangfireController : ControllerBase
{
    private readonly IConfiguration _configuration;

    public HangfireController(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    /// <summary>
    /// Encola el job Fire-and-Forget (Paso 4)
    /// </summary>
    [HttpPost("fire-and-forget")]
    public ActionResult<string> RunFireAndForget()
    {
        BackgroundJob.Enqueue(() => new 
            NotificationService().SendNotification("usuario_inmediato")); 
            
        return Ok("Job Fire-and-Forget encolado. Revisar el dashboard /hangfire.");
    }
    
    /// <summary>
    /// Encola el job Delayed (Paso 5)
    /// </summary>
    [HttpPost("delayed")]
    public ActionResult<string> RunDelayed()
    {
        BackgroundJob.Schedule(() => new
            NotificationService().SendNotification("usuario_diferido"), TimeSpan.FromMinutes(10)); //
            
        return Ok("Job Delayed encolado. Se ejecutará en 1 minuto. Revisar el dashboard /hangfire.");
    }

    /// <summary>
    /// Encola el job Recurrent (Paso 6)
    /// </summary>
    [HttpPost("recurrent")]
    public ActionResult<string> RunRecurrent()
    {
        RecurringJob.AddOrUpdate(
            "job-notificacion-cada-5-min",
            () => new NotificationService().SendNotification("usuario_recurrente"), 
            "*/5 * * * *"); 
            
        return Ok("Job Recurrente programado. Revisar la pestaña 'Recurring Jobs' en /hangfire.");
    }
}





















