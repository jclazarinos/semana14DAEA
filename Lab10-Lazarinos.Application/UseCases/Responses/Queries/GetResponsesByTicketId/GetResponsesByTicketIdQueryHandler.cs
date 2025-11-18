using Lab10_Lazarinos.Application.DTOs.Tickets;
using Lab10_Lazarinos.Application.Interfaces.Persistence;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Responses.Queries.GetResponsesByTicketId;

public class GetResponsesByTicketIdQueryHandler : IRequestHandler<GetResponsesByTicketIdQuery, IEnumerable<ResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetResponsesByTicketIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ResponseDto>> Handle(GetResponsesByTicketIdQuery request, CancellationToken cancellationToken)
    {
        var responses = await _unitOfWork.Responses.GetResponsesByTicketIdAsync(request.TicketId);
        
        return responses.Select(r => new ResponseDto
        {
            ResponseId = r.ResponseId,
            ResponderId = r.ResponderId,
            ResponderUsername = r.Responder?.Username ?? "Unknown",
            Message = r.Message,
            CreatedAt = r.CreatedAt
        }).ToList();
    }
}