using Lab10_Lazarinos.Domain.Entities;

namespace Lab10_Lazarinos.Application.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(User user, List<string> roles);
    Guid? GetUserIdFromToken(string token);
}