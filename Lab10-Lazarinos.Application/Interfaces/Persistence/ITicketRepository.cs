using Lab10_Lazarinos.Domain.Entities;

namespace Lab10_Lazarinos.Application.Interfaces.Persistence;

public interface ITicketRepository : IGenericRepository<Ticket>
{
    Task<IEnumerable<Ticket>> GetTicketsByUserIdAsync(Guid userId);
    Task<IEnumerable<Ticket>> GetTicketsByStatusAsync(string status);
    Task<Ticket?> GetTicketWithResponsesAsync(Guid ticketId);
    Task<IEnumerable<Ticket>> GetOpenTicketsAsync();
}