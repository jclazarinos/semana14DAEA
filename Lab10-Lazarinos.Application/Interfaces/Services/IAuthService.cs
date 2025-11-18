using Lab10_Lazarinos.Application.DTOs.Auth;

namespace Lab10_Lazarinos.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request);
    Task<AuthResponseDto> LoginAsync(LoginRequestDto request);
    Task<bool> AssignRoleToUserAsync(Guid userId, string roleName);
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}