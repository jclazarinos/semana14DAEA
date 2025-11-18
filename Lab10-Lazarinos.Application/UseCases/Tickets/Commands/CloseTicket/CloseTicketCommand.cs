using Lab10_Lazarinos.Application.DTOs.Tickets;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Tickets.Commands.CloseTicket;

public class CloseTicketCommand : IRequest<TicketResponseDto>
{
    public Guid TicketId { get; set; }
}