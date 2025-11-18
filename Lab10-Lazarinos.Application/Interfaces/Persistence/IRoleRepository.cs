using Lab10_Lazarinos.Domain.Entities;

namespace Lab10_Lazarinos.Application.Interfaces.Persistence;

public interface IRoleRepository : IGenericRepository<Role>
{
    Task<Role?> GetByNameAsync(string roleName);
}