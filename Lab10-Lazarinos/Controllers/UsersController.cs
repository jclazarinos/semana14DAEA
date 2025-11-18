using Lab10_Lazarinos.Application.DTOs.Common;
using Lab10_Lazarinos.Application.DTOs.Users;
using Lab10_Lazarinos.Application.UseCases.Users.Commands.UpdateUser;
using Lab10_Lazarinos.Application.UseCases.Users.Commands.DeleteUser;
using Lab10_Lazarinos.Application.UseCases.Users.Commands.ChangePassword;
using Lab10_Lazarinos.Application.UseCases.Users.Queries.GetAllUsers;
using Lab10_Lazarinos.Application.UseCases.Users.Queries.GetUserById;
using Lab10_Lazarinos.Application.UseCases.Users.Queries.GetUserByUsername;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Lab10_Lazarinos.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Obtener todos los usuarios (Solo Admin)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<IEnumerable<UserResponseDto>>>> GetAllUsers()
    {
        try
        {
            var query = new GetAllUsersQuery();
            var result = await _mediator.Send(query);
            
            return Ok(ApiResponse<IEnumerable<UserResponseDto>>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<UserResponseDto>>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Obtener usuario por ID
    /// </summary>
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> GetUserById(Guid id)
    {
        try
        {
            var query = new GetUserByIdQuery { UserId = id };
            var result = await _mediator.Send(query);
            
            return Ok(ApiResponse<UserResponseDto>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            return NotFound(ApiResponse<UserResponseDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Obtener usuario por username
    /// </summary>
    [HttpGet("by-username/{username}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> GetUserByUsername(string username)
    {
        try
        {
            var query = new GetUserByUsernameQuery { Username = username };
            var result = await _mediator.Send(query);
            
            return Ok(ApiResponse<UserResponseDto>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            return NotFound(ApiResponse<UserResponseDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Actualizar información del usuario
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> UpdateUser(
        Guid id, 
        [FromBody] UpdateUserDto dto)
    {
        try
        {
            var currentUserId = GetCurrentUserId();
            var isAdmin = User.IsInRole("Admin");

            if (id != currentUserId && !isAdmin)
                return Forbid();

            var command = new UpdateUserCommand
            {
                UserId = id,
                Email = dto.Email,
                NewPassword = dto.NewPassword
            };

            var result = await _mediator.Send(command);
            
            return Ok(ApiResponse<UserResponseDto>.SuccessResponse(result, "User updated successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<UserResponseDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Cambiar contraseña del usuario autenticado
    /// </summary>
    [HttpPost("change-password")]
    public async Task<ActionResult<ApiResponse<bool>>> ChangePassword([FromBody] ChangePasswordDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            var command = new ChangePasswordCommand
            {
                UserId = userId,
                CurrentPassword = dto.CurrentPassword,
                NewPassword = dto.NewPassword
            };

            var result = await _mediator.Send(command);
            
            return Ok(ApiResponse<bool>.SuccessResponse(result, "Password changed successfully"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Eliminar usuario (Solo Admin)
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteUser(Guid id)
    {
        try
        {
            var command = new DeleteUserCommand { UserId = id };
            var result = await _mediator.Send(command);
            
            if (result)
                return Ok(ApiResponse<bool>.SuccessResponse(true, "User deleted successfully"));
            
            return NotFound(ApiResponse<bool>.ErrorResponse("User not found"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
    }

    private Guid GetCurrentUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            throw new UnauthorizedAccessException("Invalid user token");
        
        return userId;
    }
}