using Lab10_Lazarinos.Application.DTOs.Tickets;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Tickets.Queries.GetMyTickets;

public class GetMyTicketsQuery : IRequest<IEnumerable<TicketResponseDto>>
{
    public Guid UserId { get; set; }
}