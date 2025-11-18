using Lab10_Lazarinos.Application.DTOs.Tickets;
using Lab10_Lazarinos.Application.Interfaces.Persistence;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Tickets.Commands.CloseTicket;

public class CloseTicketCommandHandler : IRequestHandler<CloseTicketCommand, TicketResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CloseTicketCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TicketResponseDto> Handle(CloseTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(request.TicketId);
        
        if (ticket == null)
            throw new Exception("Ticket not found");

        ticket.Status = "closed";
        ticket.ClosedAt = DateTime.UtcNow;

        _unitOfWork.Tickets.Update(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var closedTicket = await _unitOfWork.Tickets.GetTicketWithResponsesAsync(ticket.TicketId);

        return new TicketResponseDto
        {
            TicketId = closedTicket!.TicketId,
            UserId = closedTicket.UserId,
            Username = closedTicket.User?.Username ?? "Unknown",
            Title = closedTicket.Title,
            Description = closedTicket.Description,
            Status = closedTicket.Status,
            CreatedAt = closedTicket.CreatedAt,
            ClosedAt = closedTicket.ClosedAt,
            Responses = closedTicket.Responses?.Select(r => new ResponseDto
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