using Lab10_Lazarinos.Application.DTOs.Tickets;
using Lab10_Lazarinos.Application.Interfaces.Persistence;
using Lab10_Lazarinos.Application.Interfaces.Services;
using Lab10_Lazarinos.Domain.Entities;

namespace Lab10_Lazarinos.Infrastructure.Services;

public class TicketService : ITicketService
{
    private readonly IUnitOfWork _unitOfWork;

    public TicketService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<TicketResponseDto> CreateTicketAsync(CreateTicketDto dto, Guid userId)
    {
        // Verificar que el usuario existe
        var userExists = await _unitOfWork.Users.GetByIdAsync(userId);
        if (userExists == null)
            throw new Exception("User not found");

        var ticket = new Ticket
        {
            TicketId = Guid.NewGuid(),
            UserId = userId,
            Title = dto.Title,
            Description = dto.Description,
            Status = "abierto",
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Tickets.AddAsync(ticket);
        await _unitOfWork.SaveChangesAsync();

        // Obtener el ticket recién creado con sus relaciones
        return await GetTicketByIdAsync(ticket.TicketId);
    }

    public async Task<TicketResponseDto> GetTicketByIdAsync(Guid ticketId)
    {
        var ticket = await _unitOfWork.Tickets.GetTicketWithResponsesAsync(ticketId);
        
        if (ticket == null)
            throw new Exception("Ticket not found");

        return MapToDto(ticket);
    }

    public async Task<IEnumerable<TicketResponseDto>> GetUserTicketsAsync(Guid userId)
    {
        var tickets = await _unitOfWork.Tickets.GetTicketsByUserIdAsync(userId);
        return tickets.Select(MapToDto);
    }

    public async Task<IEnumerable<TicketResponseDto>> GetAllTicketsAsync()
    {
        var tickets = await _unitOfWork.Tickets.GetAllAsync();
        return tickets.Select(MapToDto);
    }

    public async Task<TicketResponseDto> UpdateTicketAsync(Guid ticketId, UpdateTicketDto dto)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(ticketId);
        
        if (ticket == null)
            throw new Exception("Ticket not found");

        if (!string.IsNullOrEmpty(dto.Title))
            ticket.Title = dto.Title;

        if (dto.Description != null)
            ticket.Description = dto.Description;

        if (!string.IsNullOrEmpty(dto.Status))
        {
            ticket.Status = dto.Status;
            if (dto.Status == "cerrado")
                ticket.ClosedAt = DateTime.UtcNow;
        }

        _unitOfWork.Tickets.Update(ticket);
        await _unitOfWork.SaveChangesAsync();

        return await GetTicketByIdAsync(ticketId);
    }

    public async Task<bool> DeleteTicketAsync(Guid ticketId)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(ticketId);
        
        if (ticket == null)
            return false;

        _unitOfWork.Tickets.Remove(ticket);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<ResponseDto> AddResponseToTicketAsync(CreateResponseDto dto, Guid responderId)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(dto.TicketId);
        
        if (ticket == null)
            throw new Exception("Ticket not found");

        var response = new Response
        {
            ResponseId = Guid.NewGuid(),
            TicketId = dto.TicketId,
            ResponderId = responderId,
            Message = dto.Message,
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Responses.AddAsync(response);
        await _unitOfWork.SaveChangesAsync();

        var responder = await _unitOfWork.Users.GetByIdAsync(responderId);

        return new ResponseDto
        {
            ResponseId = response.ResponseId,
            ResponderId = responderId,
            ResponderUsername = responder?.Username ?? "Unknown",
            Message = response.Message,
            CreatedAt = response.CreatedAt
        };
    }
    
    // Agregar estos métodos al archivo TicketService.cs existente

    public async Task<IEnumerable<TicketResponseDto>> GetTicketsByStatusAsync(string status)
    {
        var tickets = await _unitOfWork.Tickets.GetTicketsByStatusAsync(status);
        return tickets.Select(MapToDto);
    }

    public async Task<TicketResponseDto> CloseTicketAsync(Guid ticketId)
    {
        var ticket = await _unitOfWork.Tickets.GetByIdAsync(ticketId);
    
        if (ticket == null)
            throw new Exception("Ticket not found");

        ticket.Status = "cerrado";
        ticket.ClosedAt = DateTime.UtcNow;

        _unitOfWork.Tickets.Update(ticket);
        await _unitOfWork.SaveChangesAsync();

        return await GetTicketByIdAsync(ticketId);
    }

    private TicketResponseDto MapToDto(Ticket ticket)
    {
        return new TicketResponseDto
        {
            TicketId = ticket.TicketId,
            UserId = ticket.UserId,
            Username = ticket.User?.Username ?? "Unknown",
            Title = ticket.Title,
            Description = ticket.Description,
            Status = ticket.Status,
            CreatedAt = ticket.CreatedAt,
            ClosedAt = ticket.ClosedAt,
            Responses = ticket.Responses?.Select(r => new ResponseDto
            {
                ResponseId = r.ResponseId,
                ResponderId = r.ResponderId,
                ResponderUsername = r.Responder?.Username ?? "Unknown",
                Message = r.Message,
                CreatedAt = r.CreatedAt
            }).ToList() ?? new List<ResponseDto>()
        };
    }
}