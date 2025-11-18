using Lab10_Lazarinos.Application.DTOs.Tickets;
using Lab10_Lazarinos.Application.Interfaces.Persistence;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Tickets.Commands.UpdateTicket;

public class UpdateTicketCommandHandler : IRequestHandler<UpdateTicketCommand, TicketResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateTicketCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TicketResponseDto> Handle(UpdateTicketCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(request.TicketId);
        
        if (ticket == null)
            throw new Exception("Ticket not found");

        if (!string.IsNullOrEmpty(request.Title))
            ticket.Title = request.Title;

        if (request.Description != null)
            ticket.Description = request.Description;

        if (!string.IsNullOrEmpty(request.Status))
        {
            ticket.Status = request.Status.ToLower().Replace(" ", "_");
            
            if (ticket.Status == "closed")
                ticket.ClosedAt = DateTime.UtcNow;
        }

        _unitOfWork.Tickets.Update(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var updatedTicket = await _unitOfWork.Tickets.GetTicketWithResponsesAsync(ticket.TicketId);

        return new TicketResponseDto
        {
            TicketId = updatedTicket!.TicketId,
            UserId = updatedTicket.UserId,
            Username = updatedTicket.User?.Username ?? "Unknown",
            Title = updatedTicket.Title,
            Description = updatedTicket.Description,
            Status = updatedTicket.Status,
            CreatedAt = updatedTicket.CreatedAt,
            ClosedAt = updatedTicket.ClosedAt,
            Responses = updatedTicket.Responses?.Select(r => new ResponseDto
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