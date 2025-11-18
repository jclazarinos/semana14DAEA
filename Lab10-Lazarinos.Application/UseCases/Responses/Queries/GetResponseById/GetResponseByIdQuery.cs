using Lab10_Lazarinos.Application.DTOs.Tickets;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Responses.Queries.GetResponseById;

public class GetResponseByIdQuery : IRequest<ResponseDto>
{
    public Guid ResponseId { get; set; }
}