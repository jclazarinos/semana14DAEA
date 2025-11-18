using Lab10_Lazarinos.Application.DTOs.Auth;
using Lab10_Lazarinos.Application.Interfaces.Persistence;
using Lab10_Lazarinos.Application.Interfaces.Services;
using Lab10_Lazarinos.Domain.Entities;

namespace Lab10_Lazarinos.Infrastructure.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;

    public AuthService(IUnitOfWork unitOfWork, IJwtService jwtService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterRequestDto request)
    {
        // Validar si el usuario ya existe
        if (await _unitOfWork.Users.ExistsByUsernameAsync(request.Username))
            throw new Exception("Username already exists");

        if (await _unitOfWork.Users.ExistsByEmailAsync(request.Email))
            throw new Exception("Email already exists");

        // Crear nuevo usuario
        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync(); 

        // Asignar rol por defecto "User"
        var userRole = await _unitOfWork.Roles.GetByNameAsync("User");
        if (userRole != null)
        {
            var userRoleAssignment = new UserRole
            {
                UserId = user.UserId,
                RoleId = userRole.RoleId,
                AssignedAt = DateTime.UtcNow
            };
        
            await _unitOfWork.UserRoles.AddAsync(userRoleAssignment); // ✅ CORREGIDO
            await _unitOfWork.SaveChangesAsync();
        }

        // Obtener usuario con roles
        var userWithRoles = await _unitOfWork.Users.GetUserWithRolesAsync(user.UserId);
        var roles = userWithRoles?.UserRoles.Select(ur => ur.Role.RoleName).ToList() ?? new List<string>();

        // Generar token JWT
        var token = _jwtService.GenerateToken(user, roles);

        return new AuthResponseDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email ?? string.Empty,
            Token = token,
            Roles = roles
        };
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequestDto request)
    {
        // Buscar usuario
        var user = await _unitOfWork.Users.GetByUsernameAsync(request.Username);
        
        if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

        // Obtener roles del usuario
        var userWithRoles = await _unitOfWork.Users.GetUserWithRolesAsync(user.UserId);
        var roles = userWithRoles?.UserRoles.Select(ur => ur.Role.RoleName).ToList() ?? new List<string>();

        // Generar token JWT
        var token = _jwtService.GenerateToken(user, roles);

        return new AuthResponseDto
        {
            UserId = user.UserId,
            Username = user.Username,
            Email = user.Email ?? string.Empty,
            Token = token,
            Roles = roles
        };
    }

    public async Task<bool> AssignRoleToUserAsync(Guid userId, string roleName)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(userId);
        var role = await _unitOfWork.Roles.GetByNameAsync(roleName);

        if (user == null || role == null)
            return false;

        var userRole = new UserRole
        {
            UserId = userId,
            RoleId = role.RoleId,
            AssignedAt = DateTime.UtcNow
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}