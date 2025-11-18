namespace Lab10_Lazarinos.Application.Interfaces.Services;

// Define un DTO genérico o uno específico para tus reportes
// Por ejemplo, en Lab10-Lazarinos.Application\DTOs\Tickets\TicketReportDto.cs
public record TicketReportDto(int Id, string Title, string Status, DateTime CreatedAt);


public interface IExcelGeneratorService
    {
        // Usamos byte[] para que la capa de aplicación no sepa nada
        // del sistema de archivos o de respuestas HTTP.
        byte[] GenerateTicketReport(IEnumerable<TicketReportDto> tickets);
        
        // Aquí agregarías tu segundo reporte
        // byte[] GenerateUserReport(IEnumerable<UserReportDto> users);
}
