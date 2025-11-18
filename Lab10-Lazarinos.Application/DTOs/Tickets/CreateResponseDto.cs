namespace Lab10_Lazarinos.Application.DTOs.Tickets;

public class CreateResponseDto
{
    public Guid TicketId { get; set; }
    public string Message { get; set; } = null!;
}