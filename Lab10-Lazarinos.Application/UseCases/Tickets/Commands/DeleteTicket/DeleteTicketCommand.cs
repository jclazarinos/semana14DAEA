using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Tickets.Commands.DeleteTicket;

public class DeleteTicketCommand : IRequest<bool>
{
    public Guid TicketId { get; set; }
}