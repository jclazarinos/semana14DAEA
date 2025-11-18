using Lab10_Lazarinos.Application.DTOs.Users;
using Lab10_Lazarinos.Application.Interfaces.Persistence;
using Lab10_Lazarinos.Application.Interfaces.Services;

namespace Lab10_Lazarinos.Infrastructure.Services;

public class UserService : IUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthService _authService;

    public UserService(IUnitOfWork unitOfWork, IAuthService authService)
    {
        _unitOfWork = unitOfWork;
        _authService = authService;
    }

    public async Task<UserResponseDto> GetUserByIdAsync(Guid userId)
    {
        var user = await _unitOfWork.Users.GetUserWithRolesAsync(userId);
        
        if (user == null)
            throw new Exception("User not found");

        return new UserResponseDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            Roles = user.UserRoles.Select(ur => ur.Role.RoleName).ToList()
        };
    }

    public async Task<UserResponseDto> GetUserByUsernameAsync(string username)
    {
        var user = await _unitOfWork.Users.GetByUsernameAsync(username);
        
        if (user == null)
            throw new Exception("User not found");

        var userWithRoles = await _unitOfWork.Users.GetUserWithRolesAsync(user.UserId);
        
        return new UserResponseDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email,
            CreatedAt = user.CreatedAt,
            Roles = userWithRoles?.UserRoles.Select(ur => ur.Role.RoleName).ToList() ?? new List<string>()
        };
    }

    public async Task<IEnumerable<UserResponseDto>> GetAllUsersAsync()
    {
        var users = await _unitOfWork.Users.GetAllAsync();
        var result = new List<UserResponseDto>();

        foreach (var user in users)
        {
            var userWithRoles = await _unitOfWork.Users.GetUserWithRolesAsync(user.UserId);
            result.Add(new UserResponseDto
            {
                UserId = user.UserId,
                Username = user.Username,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                Roles = userWithRoles?.UserRoles.Select(ur => ur.Role.RoleName).ToList() ?? new List<string>()
            });
        }

        return result;
    }

    public async Task<UserResponseDto> UpdateUserAsync(Guid userId, UpdateUserDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        
        if (user == null)
            throw new Exception("User not found");

        // Actualizar email si se proporciona
        if (!string.IsNullOrEmpty(dto.Email) && dto.Email != user.Email)
        {
            // Verificar que el email no esté en uso
            if (await _unitOfWork.Users.ExistsByEmailAsync(dto.Email))
                throw new Exception("Email already in use");
            
            user.Email = dto.Email;
        }

        // Actualizar contraseña si se proporciona
        if (!string.IsNullOrEmpty(dto.NewPassword))
        {
            user.PasswordHash = _authService.HashPassword(dto.NewPassword);
        }

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return await GetUserByIdAsync(userId);
    }

    public async Task<bool> DeleteUserAsync(Guid userId)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        
        if (user == null)
            return false;

        _unitOfWork.Users.Remove(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<bool> ChangePasswordAsync(Guid userId, ChangePasswordDto dto)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        
        if (user == null)
            throw new Exception("User not found");

        // Verificar contraseña actual
        if (!_authService.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
            throw new UnauthorizedAccessException("Current password is incorrect");

        // Actualizar contraseña
        user.PasswordHash = _authService.HashPassword(dto.NewPassword);
        
        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}