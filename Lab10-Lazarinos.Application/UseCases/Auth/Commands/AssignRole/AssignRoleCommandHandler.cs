using Lab10_Lazarinos.Application.Interfaces.Persistence;
using Lab10_Lazarinos.Domain.Entities;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Auth.Commands.AssignRole;

public class AssignRoleCommandHandler : IRequestHandler<AssignRoleCommand, bool>
{
    private readonly IUnitOfWork _unitOfWork;

    public AssignRoleCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<bool> Handle(AssignRoleCommand request, CancellationToken cancellationToken)
    {
        var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
        var role = await _unitOfWork.Roles.GetByNameAsync(request.RoleName);

        if (user == null || role == null)
            return false;

        var userRole = new UserRole
        {
            UserId = request.UserId,
            RoleId = role.RoleId,
            AssignedAt = DateTime.UtcNow
        };

        await _unitOfWork.UserRoles.AddAsync(userRole);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return true;
    }
}