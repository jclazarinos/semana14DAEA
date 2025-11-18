using Lab10_Lazarinos.Application.DTOs.Users;
using Lab10_Lazarinos.Application.Interfaces.Persistence;
using Lab10_Lazarinos.Application.Interfaces.Services;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Users.Commands.UpdateUser;

public class UpdateUserCommandHandler : IRequestHandler<UpdateUserCommand, UserResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuthService _authService;

    public UpdateUserCommandHandler(IUnitOfWork unitOfWork, IAuthService authService)
    {
        _unitOfWork = unitOfWork;
        _authService = authService;
    }

    public async Task<UserResponseDto> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        
        if (user == null)
            throw new Exception("User not found");

        if (!string.IsNullOrEmpty(request.Email) && request.Email != user.Email)
        {
            if (await _unitOfWork.Users.ExistsByEmailAsync(request.Email))
                throw new Exception("Email already in use");
            
            user.Email = request.Email;
        }

        if (!string.IsNullOrEmpty(request.NewPassword))
        {
            user.PasswordHash = _authService.HashPassword(request.NewPassword);
        }

        _unitOfWork.Users.Update(user);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var userWithRoles = await _unitOfWork.Users.GetUserWithRolesAsync(user.UserId);

        return new UserResponseDto
        {
            UserId = userWithRoles!.UserId,
            Username = userWithRoles.Username,
            Email = userWithRoles.Email,
            CreatedAt = userWithRoles.CreatedAt,
            Roles = userWithRoles.UserRoles.Select(ur => ur.Role.RoleName).ToList()
        };
    }
}