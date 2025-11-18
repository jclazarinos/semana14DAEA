using Lab10_Lazarinos.Application.DTOs.Auth;
using Lab10_Lazarinos.Application.Interfaces.Persistence;
using Lab10_Lazarinos.Application.Interfaces.Services;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IJwtService _jwtService;
    private readonly IAuthService _authService;

    public LoginCommandHandler(IUnitOfWork unitOfWork, IJwtService jwtService, IAuthService authService)
    {
        _unitOfWork = unitOfWork;
        _jwtService = jwtService;
        _authService = authService;
    }

    public async Task<AuthResponseDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByUsernameAsync(request.Username);
        
        if (user == null || !_authService.VerifyPassword(request.Password, user.PasswordHash))
            throw new UnauthorizedAccessException("Invalid credentials");

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