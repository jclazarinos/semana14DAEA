namespace Lab10_Lazarinos.Application.Interfaces.Persistence;

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    ITicketRepository Tickets { get; }
    IRoleRepository Roles { get; }
    IResponseRepository Responses { get; }
    IUserRoleRepository UserRoles { get; }
    
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}