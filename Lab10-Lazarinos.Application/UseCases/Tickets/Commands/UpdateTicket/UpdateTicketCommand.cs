using Lab10_Lazarinos.Application.DTOs.Tickets;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Tickets.Commands.UpdateTicket;

public class UpdateTicketCommand : IRequest<TicketResponseDto>
{
    public Guid TicketId { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public string? Status { get; set; }
}