namespace Lab10_Lazarinos.Infrastructure.Services
{
    public class NotificationService
    {
        public void SendNotification(string user)
        {
            Console.WriteLine($"Simulando fallo crítico para {user} en {DateTime.Now}"); 
            
            throw new InvalidOperationException($"ERROR INTENCIONAL: No se pudo conectar al servicio de notificación externa para {user}.");
            // La lógica del job
           // Console.WriteLine($"Notificación enviada a {user} en {DateTime.Now}"); // 
        }
    }
}
