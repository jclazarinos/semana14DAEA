using Lab10_Lazarinos.Application.DTOs.Tickets;
using Lab10_Lazarinos.Application.Interfaces.Persistence;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Tickets.Queries.GetAllTickets;

public class GetAllTicketsQueryHandler : IRequestHandler<GetAllTicketsQuery, IEnumerable<TicketResponseDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllTicketsQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<TicketResponseDto>> Handle(GetAllTicketsQuery request, CancellationToken cancellationToken)
    {
        var tickets = await _unitOfWork.Tickets.GetAllAsync();

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