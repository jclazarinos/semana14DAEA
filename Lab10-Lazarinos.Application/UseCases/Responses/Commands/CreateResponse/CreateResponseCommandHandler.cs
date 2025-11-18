using Lab10_Lazarinos.Application.DTOs.Tickets;
using Lab10_Lazarinos.Application.Interfaces.Persistence;
using Lab10_Lazarinos.Domain.Entities;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Responses.Commands.CreateResponse;

public class CreateResponseCommandHandler : IRequestHandler<CreateResponseCommand, ResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateResponseCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseDto> Handle(CreateResponseCommand request, CancellationToken cancellationToken)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(request.TicketId);
        
        if (ticket == null)
            throw new Exception("Ticket not found");

        var response = new Response
        {
            ResponseId = Guid.NewGuid(),
            TicketId = request.TicketId,
            ResponderId = request.ResponderId,
            Message = request.Message,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Responses.AddAsync(response);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var responder = await _unitOfWork.Users.GetByIdAsync(request.ResponderId);

        return new ResponseDto
        {
            ResponseId = response.ResponseId,
            ResponderId = request.ResponderId,
            ResponderUsername = responder?.Username ?? "Unknown",
            Message = response.Message,
            CreatedAt = response.CreatedAt
        };
    }
}