using Lab10_Lazarinos.Application.DTOs.Auth;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Auth.Commands.Login;

public class LoginCommand : IRequest<AuthResponseDto>
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}