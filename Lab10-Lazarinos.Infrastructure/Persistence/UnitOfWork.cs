using Lab10_Lazarinos.Application.Interfaces.Persistence;
using Lab10_Lazarinos.Infrastructure.Repositories;
using Lab10_Lazarinos.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore.Storage;

namespace Lab10_Lazarinos.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly TicketeraDbContext _context;
    private IDbContextTransaction? _transaction;

    public IUserRepository Users { get; }
    public ITicketRepository Tickets { get; }
    public IRoleRepository Roles { get; }
    public IResponseRepository Responses { get; }
    public IUserRoleRepository UserRoles { get; }

    public UnitOfWork(TicketeraDbContext context)
    {
        _context = context;
        Users = new UserRepository(_context);
        Tickets = new TicketRepository(_context);
        Roles = new RoleRepository(_context);
        Responses = new ResponseRepository(_context);
        UserRoles = new UserRoleRepository(_context); 
    }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        try
        {
            await SaveChangesAsync();
            if (_transaction != null)
                await _transaction.CommitAsync();
        }
        catch
        {
            await RollbackTransactionAsync();
            throw;
        }
        finally
        {
            if (_transaction != null)
            {
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}