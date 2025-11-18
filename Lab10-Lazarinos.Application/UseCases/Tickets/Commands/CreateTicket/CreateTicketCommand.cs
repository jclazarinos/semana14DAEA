using Lab10_Lazarinos.Application.DTOs.Tickets;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Tickets.Commands.CreateTicket;

public class CreateTicketCommand : IRequest<TicketResponseDto>
{
    public Guid UserId { get; set; }
    public string Title { get; set; } = null!;
    public string? Description { get; set; }
}