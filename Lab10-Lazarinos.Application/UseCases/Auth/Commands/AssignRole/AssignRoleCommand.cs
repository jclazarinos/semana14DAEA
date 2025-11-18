using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Auth.Commands.AssignRole;

public class AssignRoleCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
    public string RoleName { get; set; } = null!;
}