namespace Lab10_Lazarinos.Application.DTOs.Tickets;

public class CreateTicketDto
{
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
}