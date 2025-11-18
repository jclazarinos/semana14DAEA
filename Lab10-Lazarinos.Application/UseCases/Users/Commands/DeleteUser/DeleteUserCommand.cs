using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Users.Commands.DeleteUser;

public class DeleteUserCommand : IRequest<bool>
{
    public Guid UserId { get; set; }
}