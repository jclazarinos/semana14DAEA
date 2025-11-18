using Lab10_Lazarinos.Application.DTOs.Common;
using Lab10_Lazarinos.Application.DTOs.Tickets;
using Lab10_Lazarinos.Application.UseCases.Tickets.Commands.CreateTicket;
using Lab10_Lazarinos.Application.UseCases.Tickets.Commands.UpdateTicket;
using Lab10_Lazarinos.Application.UseCases.Tickets.Commands.DeleteTicket;
using Lab10_Lazarinos.Application.UseCases.Tickets.Commands.CloseTicket;
using Lab10_Lazarinos.Application.UseCases.Tickets.Queries.GetTicketById;
using Lab10_Lazarinos.Application.UseCases.Tickets.Queries.GetAllTickets;
using Lab10_Lazarinos.Application.UseCases.Tickets.Queries.GetMyTickets;
using Lab10_Lazarinos.Application.UseCases.Tickets.Queries.GetTicketsByStatus;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Lab10_Lazarinos.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class TicketsController : ControllerBase
{
    private readonly IMediator _mediator;

    public TicketsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    /// <summary>
    /// Crear un nuevo ticket
    /// </summary>
    [HttpPost]
    public async Task<ActionResult<ApiResponse<TicketResponseDto>>> CreateTicket([FromBody] CreateTicketDto dto)
    {
        try
        {
            var userId = GetCurrentUserId();
            
            var command = new CreateTicketCommand
            {
                UserId = userId,
                Title = dto.Title,
                Description = dto.Description
            };

            var result = await _mediator.Send(command);
            
            return CreatedAtAction(nameof(GetTicketById), new { id = result.TicketId }, 
                ApiResponse<TicketResponseDto>.SuccessResponse(result, "Ticket created successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<TicketResponseDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Obtener un ticket por ID
    /// </summary>
    [HttpGet("{id}")]
    public async Task<ActionResult<ApiResponse<TicketResponseDto>>> GetTicketById(Guid id)
    {
        try
        {
            var query = new GetTicketByIdQuery { TicketId = id };
            var result = await _mediator.Send(query);
            
            return Ok(ApiResponse<TicketResponseDto>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            return NotFound(ApiResponse<TicketResponseDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Obtener todos los tickets del usuario autenticado
    /// </summary>
    [HttpGet("my-tickets")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TicketResponseDto>>>> GetMyTickets()
    {
        try
        {
            var userId = GetCurrentUserId();
            var query = new GetMyTicketsQuery { UserId = userId };
            var result = await _mediator.Send(query);
            
            return Ok(ApiResponse<IEnumerable<TicketResponseDto>>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<TicketResponseDto>>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Obtener todos los tickets (Solo Admin/Support)
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Admin,Support")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TicketResponseDto>>>> GetAllTickets()
    {
        try
        {
            var query = new GetAllTicketsQuery();
            var result = await _mediator.Send(query);
            
            return Ok(ApiResponse<IEnumerable<TicketResponseDto>>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<TicketResponseDto>>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Obtener tickets por estado
    /// </summary>
    [HttpGet("by-status/{status}")]
    [Authorize(Roles = "Admin,Support")]
    public async Task<ActionResult<ApiResponse<IEnumerable<TicketResponseDto>>>> GetTicketsByStatus(string status)
    {
        try
        {
            var query = new GetTicketsByStatusQuery { Status = status };
            var result = await _mediator.Send(query);
            
            return Ok(ApiResponse<IEnumerable<TicketResponseDto>>.SuccessResponse(result));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<IEnumerable<TicketResponseDto>>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Actualizar un ticket
    /// </summary>
    [HttpPut("{id}")]
    public async Task<ActionResult<ApiResponse<TicketResponseDto>>> UpdateTicket(
        Guid id, 
        [FromBody] UpdateTicketDto dto)
    {
        try
        {
            var command = new UpdateTicketCommand
            {
                TicketId = id,
                Title = dto.Title,
                Description = dto.Description,
                Status = dto.Status
            };

            var result = await _mediator.Send(command);
            
            return Ok(ApiResponse<TicketResponseDto>.SuccessResponse(result, "Ticket updated successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<TicketResponseDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Cerrar un ticket
    /// </summary>
    [HttpPost("{id}/close")]
    [Authorize(Roles = "Admin,Support")]
    public async Task<ActionResult<ApiResponse<TicketResponseDto>>> CloseTicket(Guid id)
    {
        try
        {
            var command = new CloseTicketCommand { TicketId = id };
            var result = await _mediator.Send(command);
            
            return Ok(ApiResponse<TicketResponseDto>.SuccessResponse(result, "Ticket closed successfully"));
        }
        catch (Exception ex)
        {
            return BadRequest(ApiResponse<TicketResponseDto>.ErrorResponse(ex.Message));
        }
    }

    /// <summary>
    /// Eliminar un ticket
    /// </summary>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<ApiResponse<bool>>> DeleteTicket(Guid id)
    {
        try
        {
            var command = new DeleteTicketCommand { TicketId = id };
            var result = await _mediator.Send(command);
            
            if (result)
                return Ok(ApiResponse<bool>.SuccessResponse(true, "Ticket deleted successfully"));
            
            return NotFound(ApiResponse<bool>.ErrorResponse("Ticket not found"));
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