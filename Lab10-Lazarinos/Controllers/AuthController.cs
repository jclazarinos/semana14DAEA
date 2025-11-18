using Lab10_Lazarinos.Application.DTOs.Auth;
using Lab10_Lazarinos.Application.DTOs.Common;
using Lab10_Lazarinos.Application.DTOs.Users;
using Lab10_Lazarinos.Application.UseCases.Auth.Commands.Register;
using Lab10_Lazarinos.Application.UseCases.Auth.Commands.Login;
using Lab10_Lazarinos.Application.UseCases.Auth.Commands.AssignRole;
using Lab10_Lazarinos.Application.UseCases.Auth.Queries.GetCurrentUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Lab10_Lazarinos.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Registrar un nuevo usuario
    /// </summary>
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Register([FromBody] RegisterRequestDto request)
    {
        try
        {
            var command = new RegisterCommand
            {
                Username = request.Username,
                Email = request.Email,
                Password = request.Password
            };

            var result = await _mediator.Send(command);
            
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "User registered successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Login de usuario
    /// </summary>
    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<ApiResponse<AuthResponseDto>>> Login([FromBody] LoginRequestDto request)
    {
        try
        {
            var command = new LoginCommand
            {
                Username = request.Username,
                Password = request.Password
            };

            var result = await _mediator.Send(command);
            
            return Ok(ApiResponse<AuthResponseDto>.SuccessResponse(result, "Login successful"));
        }
        catch (UnauthorizedAccessException ex)
        {
            return Unauthorized(ApiResponse<AuthResponseDto>.ErrorResponse(ex.Message));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<AuthResponseDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Asignar rol a un usuario (Solo Admin)
    /// </summary>
    [HttpPost("assign-role")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> AssignRole(
        [FromQuery] Guid userId, 
        [FromQuery] string roleName)
    {
        try
        {
            var command = new AssignRoleCommand
            {
                UserId = userId,
                RoleName = roleName
            };

            var result = await _mediator.Send(command);
            
            if (result)
                return Ok(ApiResponse<bool>.SuccessResponse(true, "Role assigned successfully"));
            
            return BadRequest(ApiResponse<bool>.ErrorResponse("Failed to assign role"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<bool>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Obtener información del usuario autenticado
    /// </summary>
    [HttpGet("me")]
    [Authorize]
    public async Task<ActionResult<ApiResponse<UserResponseDto>>> GetCurrentUser()
    {
        try
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
                return Unauthorized(ApiResponse<UserResponseDto>.ErrorResponse("Invalid user token"));

            var query = new GetCurrentUserQuery { UserId = userId };
            var result = await _mediator.Send(query);
            
            return Ok(ApiResponse<UserResponseDto>.SuccessResponse(result, "User information retrieved"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<UserResponseDto>.ErrorResponse(ex.Message));
        }
    }
}