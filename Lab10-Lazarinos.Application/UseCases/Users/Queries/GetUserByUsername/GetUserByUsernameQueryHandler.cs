using Lab10_Lazarinos.Application.DTOs.Users;
using Lab10_Lazarinos.Application.Interfaces.Persistence;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Users.Queries.GetUserByUsername;

public class GetUserByUsernameQueryHandler : IRequestHandler<GetUserByUsernameQuery, UserResponseDto>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetUserByUsernameQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<UserResponseDto> Handle(GetUserByUsernameQuery request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByUsernameAsync(request.Username);
        
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
}