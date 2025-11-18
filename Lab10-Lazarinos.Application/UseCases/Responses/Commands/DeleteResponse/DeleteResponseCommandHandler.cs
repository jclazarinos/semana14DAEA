using Lab10_Lazarinos.Application.Interfaces.Persistence;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Responses.Commands.DeleteResponse;

public class DeleteResponseCommandHandler : IRequestHandler<DeleteResponseCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteResponseCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(DeleteResponseCommand request, CancellationToken cancellationToken)
    {
        var response = await _unitOfWork.Responses.GetByIdAsync(request.ResponseId);
        
        if (response == null)
            return false;

        if (response.ResponderId != request.UserId)
            throw new UnauthorizedAccessException("You can only delete your own responses");

        _unitOfWork.Responses.Remove(response);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}