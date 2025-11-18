using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Users.Commands.ChangePassword;

public class ChangePasswordCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
    public string CurrentPassword { get; set; } = null!;
    public string NewPassword { get; set; } = null!;
}