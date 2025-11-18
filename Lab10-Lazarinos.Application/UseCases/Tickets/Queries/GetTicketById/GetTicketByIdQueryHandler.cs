using Lab10_Lazarinos.Application.DTOs.Tickets;
using Lab10_Lazarinos.Application.Interfaces.Persistence;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Tickets.Queries.GetTicketById;

public class GetTicketByIdQueryHandler : IRequestHandler<GetTicketByIdQuery, TicketResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTicketByIdQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TicketResponseDto> Handle(GetTicketByIdQuery request, CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.Tickets.GetTicketWithResponsesAsync(request.TicketId);
        
        if (ticket == null)
            throw new Exception("Ticket not found");

        return new TicketResponseDto
        {
            TicketId = ticket.TicketId,
            UserId = ticket.UserId,
            Username = ticket.User?.Username ?? "Unknown",
            Title = ticket.Title,
            Description = ticket.Description,
            Status = ticket.Status,
            CreatedAt = ticket.CreatedAt,
            ClosedAt = ticket.ClosedAt,
            Responses = ticket.Responses?.Select(r => new ResponseDto
            {
                ResponseId = r.ResponseId,
                ResponderId = r.ResponderId,
                ResponderUsername = r.Responder?.Username ?? "Unknown",
                Message = r.Message,
                CreatedAt = r.CreatedAt
            }).ToList() ?? new List<ResponseDto>()
        };
    }
}