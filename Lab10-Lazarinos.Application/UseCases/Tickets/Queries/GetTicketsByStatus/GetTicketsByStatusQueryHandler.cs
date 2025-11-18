using Lab10_Lazarinos.Application.DTOs.Tickets;
using Lab10_Lazarinos.Application.Interfaces.Persistence;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Tickets.Queries.GetTicketsByStatus;

public class GetTicketsByStatusQueryHandler : IRequestHandler<GetTicketsByStatusQuery, IEnumerable<TicketResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTicketsByStatusQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<TicketResponseDto>> Handle(GetTicketsByStatusQuery request, CancellationToken cancellationToken)
    {
        var normalizedStatus = request.Status.ToLower().Replace(" ", "_");
        var tickets = await _unitOfWork.Tickets.GetTicketsByStatusAsync(normalizedStatus);

        return tickets.Select(ticket => new TicketResponseDto
        {
            TicketId = ticket.TicketId,
            UserId = ticket.UserId,
            Username = ticket.User?.Username ?? "Unknown",
            Title = ticket.Title,
            Description = ticket.Description,
            Status = ticket.Status,
            CreatedAt = ticket.CreatedAt,
            ClosedAt = ticket.ClosedAt,
            Responses = new List<ResponseDto>()
        }).ToList();
    }
}