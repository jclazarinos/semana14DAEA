using Lab10_Lazarinos.Application.DTOs.Tickets;
using Lab10_Lazarinos.Application.Interfaces.Persistence;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Responses.Commands.UpdateResponse;

public class UpdateResponseCommandHandler : IRequestHandler<UpdateResponseCommand, ResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateResponseCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseDto> Handle(UpdateResponseCommand request, CancellationToken cancellationToken)
    {
        var response = await _unitOfWork.Responses.GetByIdAsync(request.ResponseId);
        
        if (response == null)
            throw new Exception("Response not found");

        if (response.ResponderId != request.UserId)
            throw new UnauthorizedAccessException("You can only edit your own responses");

        response.Message = request.NewMessage;

        _unitOfWork.Responses.Update(response);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var responder = await _unitOfWork.Users.GetByIdAsync(response.ResponderId);

        return new ResponseDto
        {
            ResponseId = response.ResponseId,
            ResponderId = response.ResponderId,
            ResponderUsername = responder?.Username ?? "Unknown",
            Message = response.Message,
            CreatedAt = response.CreatedAt
        };
    }
}