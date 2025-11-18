using Lab10_Lazarinos.Application.DTOs.Common;
using Lab10_Lazarinos.Application.DTOs.Tickets;
using Lab10_Lazarinos.Application.UseCases.Responses.Commands.CreateResponse;
using Lab10_Lazarinos.Application.UseCases.Responses.Commands.UpdateResponse;
using Lab10_Lazarinos.Application.UseCases.Responses.Commands.DeleteResponse;
using Lab10_Lazarinos.Application.UseCases.Responses.Queries.GetResponseById;
using Lab10_Lazarinos.Application.UseCases.Responses.Queries.GetResponsesByTicketId;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Lab10_Lazarinos.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ResponsesController : ControllerBase
{
    private readonly IMediator _mediator;

    public ResponsesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Crear una respuesta a un ticket
    /// </summary>
    [HttpPost]
    [Authorize(Roles = "Admin,Support")]
    public async Task<ActionResult<ApiResponse<ResponseDto>>> CreateResponse([FromBody] CreateResponseDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            var command = new CreateResponseCommand
            {
                TicketId = dto.TicketId,
                ResponderId = userId,
                Message = dto.Message
            };

            var result = await _mediator.Send(command);
            
            return Ok(ApiResponse<ResponseDto>.SuccessResponse(result, "Response created successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ResponseDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Obtener una respuesta por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<ResponseDto>>> GetResponseById(Guid id)
    {
        try
        {
            var query = new GetResponseByIdQuery { ResponseId = id };
            var result = await _mediator.Send(query);
            
            return Ok(ApiResponse<ResponseDto>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            return NotFound(ApiResponse<ResponseDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Obtener todas las respuestas de un ticket
    /// </summary>
    [HttpGet("ticket/{ticketId}")]
    public async Task<ActionResult<ApiResponse<IEnumerable<ResponseDto>>>> GetResponsesByTicket(Guid ticketId)
    {
        try
        {
            var query = new GetResponsesByTicketIdQuery { TicketId = ticketId };
            var result = await _mediator.Send(query);
            
            return Ok(ApiResponse<IEnumerable<ResponseDto>>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<ResponseDto>>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Actualizar una respuesta
    /// </summary>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin,Support")]
    public async Task<ActionResult<ApiResponse<ResponseDto>>> UpdateResponse(
        Guid id, 
        [FromBody] string newMessage)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            var command = new UpdateResponseCommand
            {
                ResponseId = id,
                UserId = userId,
                NewMessage = newMessage
            };

            var result = await _mediator.Send(command);
            
            return Ok(ApiResponse<ResponseDto>.SuccessResponse(result, "Response updated successfully"));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<ResponseDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Eliminar una respuesta
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin,Support")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteResponse(Guid id)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            var command = new DeleteResponseCommand
            {
                ResponseId = id,
                UserId = userId
            };

            var result = await _mediator.Send(command);
            
            if (result)
                return Ok(ApiResponse<bool>.SuccessResponse(true, "Response deleted successfully"));
            
            return NotFound(ApiResponse<bool>.ErrorResponse("Response not found"));
        }
        catch (UnauthorizedAccessException)
        {
            return Forbid();
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