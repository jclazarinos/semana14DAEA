using Lab10_Lazarinos.Application.DTOs.Auth;
using Lab10_Lazarinos.Application.Interfaces.Persistence;
using Lab10_Lazarinos.Application.Interfaces.Services;
using Lab10_Lazarinos.Domain.Entities;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IAuthService _authService;

    public RegisterCommandHandler(IUnitOfWork unitOfWork, IJwtService jwtService, IAuthService authService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _authService = authService;
    }

    public async Task<AuthResponseDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _unitOfWork.Users.ExistsByUsernameAsync(request.Username))
            throw new Exception("Username already exists");

        if (await _unitOfWork.Users.ExistsByEmailAsync(request.Email))
            throw new Exception("Email already exists");

        var user = new User
        {
            UserId = Guid.NewGuid(),
            Username = request.Username,
            Email = request.Email,
            PasswordHash = _authService.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userRole = await _unitOfWork.Roles.GetByNameAsync("User");
        if (userRole != null)
        {
            var userRoleAssignment = new UserRole
            {
                UserId = user.UserId,
                RoleId = userRole.RoleId,
                AssignedAt = DateTime.UtcNow
            };
            
            await _unitOfWork.UserRoles.AddAsync(userRoleAssignment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        var userWithRoles = await _unitOfWork.Users.GetUserWithRolesAsync(user.UserId);
        var roles = userWithRoles?.UserRoles.Select(ur => ur.Role.RoleName).ToList() ?? new List<string>();

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
}