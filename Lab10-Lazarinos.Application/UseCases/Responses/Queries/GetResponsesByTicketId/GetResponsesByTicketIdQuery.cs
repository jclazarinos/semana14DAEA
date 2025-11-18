using Lab10_Lazarinos.Application.DTOs.Tickets;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Responses.Queries.GetResponsesByTicketId;

public class GetResponsesByTicketIdQuery : IRequest<IEnumerable<ResponseDto>>
{
    public Guid TicketId { get; set; }
}