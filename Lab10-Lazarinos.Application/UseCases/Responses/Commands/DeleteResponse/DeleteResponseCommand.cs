using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Responses.Commands.DeleteResponse;

public class DeleteResponseCommand : IRequest<bool>
{
    public Guid ResponseId { get; set; }
    public Guid UserId { get; set; }
}