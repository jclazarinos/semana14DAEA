using Lab10_Lazarinos.Application.DTOs.Tickets;
using Lab10_Lazarinos.Application.Interfaces.Persistence;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Responses.Queries.GetResponseById;

public class GetResponseByIdQueryHandler : IRequestHandler<GetResponseByIdQuery, ResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetResponseByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseDto> Handle(GetResponseByIdQuery request, CancellationToken cancellationToken)
    {
        var response = await _unitOfWork.Responses.GetByIdAsync(request.ResponseId);
        
        if (response == null)
            throw new Exception("Response not found");

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