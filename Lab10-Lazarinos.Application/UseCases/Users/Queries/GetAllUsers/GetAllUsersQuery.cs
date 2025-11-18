using Lab10_Lazarinos.Application.DTOs.Users;
using MediatR;

namespace Lab10_Lazarinos.Application.UseCases.Users.Queries.GetAllUsers;

public class GetAllUsersQuery : IRequest<IEnumerable<UserResponseDto>>
{
}