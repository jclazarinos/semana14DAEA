using Lab10_Lazarinos.Application.DTOs.Users;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Users.Queries.GetUserByUsername;

public class GetUserByUsernameQuery : IRequest<UserResponseDto>
{
    public string Username { get; set; } = null!;
}