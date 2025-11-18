using Lab10_Lazarinos.Application.DTOs.Users;

namespace Lab10_Lazarinos.Application.Interfaces.Services;

public interface IUserService
{
    Task<UserResponseDto> GetUserByIdAsync(Guid userId);
    Task<UserResponseDto> GetUserByUsernameAsync(string username);
    Task<IEnumerable<UserResponseDto>> GetAllUsersAsync();
    Task<UserResponseDto> UpdateUserAsync(Guid userId, UpdateUserDto dto);
    Task<bool> DeleteUserAsync(Guid userId);
    Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto dto);
}