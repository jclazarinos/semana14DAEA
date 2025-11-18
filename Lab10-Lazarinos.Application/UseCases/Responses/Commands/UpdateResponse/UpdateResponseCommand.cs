using Lab10_Lazarinos.Application.DTOs.Tickets;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Responses.Commands.UpdateResponse;

public class UpdateResponseCommand : IRequest<ResponseDto>
{
    public Guid ResponseId { get; set; }
    public Guid UserId { get; set; }
    public string NewMessage { get; set; } = null!;
}