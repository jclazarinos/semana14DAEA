using Lab10_Lazarinos.Application.Interfaces.Persistence;
using Lab10_Lazarinos.Domain.Entities;
using Lab10_Lazarinos.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Lab10_Lazarinos.Infrastructure.Repositories;

public class TicketRepository : GenericRepository<Ticket>, ITicketRepository
{
    public TicketRepository(TicketeraDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Ticket>> GetTicketsByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(t => t.UserId == userId)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Ticket>> GetTicketsByStatusAsync(string status)
    {
        return await _dbSet
            .Where(t => t.Status == status)
            .Include(t => t.User)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }

    public async Task<Ticket?> GetTicketWithResponsesAsync(Guid ticketId)
    {
        return await _dbSet
            .Include(t => t.User)
            .Include(t => t.Responses)
            .ThenInclude(r => r.Responder)
            .FirstOrDefaultAsync(t => t.TicketId == ticketId);
    }

    public async Task<IEnumerable<Ticket>> GetOpenTicketsAsync()
    {
        return await _dbSet
            .Where(t => t.Status == "abierto" || t.Status == "en_proceso")
            .Include(t => t.User)
            .OrderByDescending(t => t.CreatedAt)
            .ToListAsync();
    }
}