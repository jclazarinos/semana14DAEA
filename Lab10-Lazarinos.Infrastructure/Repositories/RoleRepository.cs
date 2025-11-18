using Lab10_Lazarinos.Application.Interfaces.Persistence;
using Lab10_Lazarinos.Domain.Entities;
using Lab10_Lazarinos.Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Lab10_Lazarinos.Infrastructure.Repositories;

public class RoleRepository : GenericRepository<Role>, IRoleRepository
{
    public RoleRepository(TicketeraDbContext context) : base(context)
    {
    }

    public async Task<Role?> GetByNameAsync(string roleName)
    {
        return await _dbSet
            .FirstOrDefaultAsync(r => r.RoleName == roleName);
    }
}