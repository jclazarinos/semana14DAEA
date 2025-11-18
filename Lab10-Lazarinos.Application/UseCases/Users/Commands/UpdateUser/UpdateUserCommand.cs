using Lab10_Lazarinos.Application.DTOs.Users;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Users.Commands.UpdateUser;

public class UpdateUserCommand : IRequest<UserResponseDto>
{
    public Guid UserId { get; set; }
    public string? Email { get; set; }
    public string? NewPassword { get; set; }
}