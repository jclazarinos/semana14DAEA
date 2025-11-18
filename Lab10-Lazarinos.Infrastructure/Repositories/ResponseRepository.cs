using Lab10_Lazarinos.Application.Interfaces.Persistence;
using Lab10_Lazarinos.Domain.Entities;
using Lab10_Lazarinos.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Lab10_Lazarinos.Infrastructure.Repositories;

public class ResponseRepository : GenericRepository<Response>, IResponseRepository
{
    public ResponseRepository(TicketeraDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Response>> GetResponsesByTicketIdAsync(Guid ticketId)
    {
        return await _dbSet
            .Where(r => r.TicketId == ticketId)
            .Include(r => r.Responder)
            .OrderBy(r => r.CreatedAt)
            .ToListAsync();
    }

    public async Task<IEnumerable<Response>> GetResponsesByResponderIdAsync(Guid responderId)
    {
        return await _dbSet
            .Where(r => r.ResponderId == responderId)
            .Include(r => r.Ticket)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync();
    }
}