namespace Lab10_Lazarinos.Application.DTOs.Tickets;

public class UpdateTicketDto
{
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; } // "Open", "In Progress", "Closed"
}