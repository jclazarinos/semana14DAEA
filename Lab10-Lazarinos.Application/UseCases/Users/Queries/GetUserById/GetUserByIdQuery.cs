using Lab10_Lazarinos.Application.DTOs.Users;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Users.Queries.GetUserById;

public class GetUserByIdQuery : IRequest<UserResponseDto>
{
    public Guid UserId { get; set; }
}