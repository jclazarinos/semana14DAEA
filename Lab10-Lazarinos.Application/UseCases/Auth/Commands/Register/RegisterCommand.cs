using Lab10_Lazarinos.Application.DTOs.Auth;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Auth.Commands.Register;

public class RegisterCommand : IRequest<AuthResponseDto>
{
    public string Username { get; set; } = null!;
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}