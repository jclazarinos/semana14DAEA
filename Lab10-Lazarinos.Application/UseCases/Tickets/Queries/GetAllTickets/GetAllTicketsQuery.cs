using Lab10_Lazarinos.Application.DTOs.Tickets;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Tickets.Queries.GetAllTickets;

public class GetAllTicketsQuery : IRequest<IEnumerable<TicketResponseDto>>
{
}