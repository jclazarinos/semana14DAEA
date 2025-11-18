using Lab10_Lazarinos.Application.DTOs.Users;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Auth.Queries.GetCurrentUser;

public class GetCurrentUserQuery : IRequest<UserResponseDto>
{
    public Guid UserId { get; set; }
}