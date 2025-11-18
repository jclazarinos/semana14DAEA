using Lab10_Lazarinos.Application.Interfaces.Persistence;
using Lab10_Lazarinos.Domain.Entities;
using Lab10_Lazarinos.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Lab10_Lazarinos.Infrastructure.Repositories;

public class UserRoleRepository : GenericRepository<UserRole>, IUserRoleRepository
{
    public UserRoleRepository(TicketeraDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<UserRole>> GetRolesByUserIdAsync(Guid userId)
    {
        return await _dbSet
            .Where(ur => ur.UserId == userId)
            .Include(ur => ur.Role)
            .ToListAsync();
    }
}