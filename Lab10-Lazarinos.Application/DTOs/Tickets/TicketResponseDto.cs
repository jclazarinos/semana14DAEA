namespace Lab10_Lazarinos.Application.DTOs.Tickets;

public class TicketResponseDto
{
    public Guid TicketId { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; } = null!;
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
    public string Status { get; set; } = null!;
    public DateTime? CreatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public List<ResponseDto> Responses { get; set; } = new();
}