using Lab10_Lazarinos.Application.DTOs.Tickets;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Tickets.Queries.GetTicketById;

public class GetTicketByIdQuery : IRequest<TicketResponseDto>
{
    public Guid TicketId { get; set; }
}