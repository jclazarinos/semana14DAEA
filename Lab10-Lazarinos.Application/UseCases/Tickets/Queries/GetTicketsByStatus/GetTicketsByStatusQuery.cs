using Lab10_Lazarinos.Application.DTOs.Tickets;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Tickets.Queries.GetTicketsByStatus;

public class GetTicketsByStatusQuery : IRequest<IEnumerable<TicketResponseDto>>
{
    public string Status { get; set; } = null!;
}