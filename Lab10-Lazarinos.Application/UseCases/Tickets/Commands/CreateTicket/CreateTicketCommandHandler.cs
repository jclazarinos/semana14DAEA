using Lab10_Lazarinos.Application.DTOs.Tickets;
using Lab10_Lazarinos.Application.Interfaces.Persistence;
using Lab10_Lazarinos.Domain.Entities;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Tickets.Commands.CreateTicket;

public class CreateTicketCommandHandler : IRequestHandler<CreateTicketCommand, TicketResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public CreateTicketCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TicketResponseDto> Handle(CreateTicketCommand request, CancellationToken cancellationToken)
    {
        // Verificar que el usuario existe
        var userExists = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        if (userExists == null)
            throw new Exception("User not found");

        // Crear el ticket
        var ticket = new Ticket
        {
            TicketId = Guid.NewGuid(),
            UserId = request.UserId,
            Title = request.Title,
            Description = request.Description,
            Status = "abierto",
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Tickets.AddAsync(ticket);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Obtener el ticket con sus relaciones
        var createdTicket = await _unitOfWork.Tickets.GetTicketWithResponsesAsync(ticket.TicketId);

        return new TicketResponseDto
        {
            TicketId = createdTicket!.TicketId,
            UserId = createdTicket.UserId,
            Username = createdTicket.User?.Username ?? "Unknown",
            Title = createdTicket.Title,
            Description = createdTicket.Description,
            Status = createdTicket.Status,
            CreatedAt = createdTicket.CreatedAt,
            ClosedAt = createdTicket.ClosedAt,
            Responses = new List<ResponseDto>()
        };
    }
}