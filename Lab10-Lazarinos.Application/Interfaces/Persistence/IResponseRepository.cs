using Lab10_Lazarinos.Domain.Entities;

namespace Lab10_Lazarinos.Application.Interfaces.Persistence;

public interface IResponseRepository : IGenericRepository<Response>
{
    Task<IEnumerable<Response>> GetResponsesByTicketIdAsync(Guid ticketId);
    Task<IEnumerable<Response>> GetResponsesByResponderIdAsync(Guid responderId);
}