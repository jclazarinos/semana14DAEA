using Lab10_Lazarinos.Application.DTOs.Tickets;

namespace Lab10_Lazarinos.Application.Interfaces.Services;

public interface ITicketService
{
    Task<TicketResponseDto> CreateTicketAsync(CreateTicketDto dto, Guid userId);
    Task<TicketResponseDto> GetTicketByIdAsync(Guid ticketId);
    Task<IEnumerable<TicketResponseDto>> GetUserTicketsAsync(Guid userId);
    Task<IEnumerable<TicketResponseDto>> GetAllTicketsAsync();
    Task<IEnumerable<TicketResponseDto>> GetTicketsByStatusAsync(string status); // ✅ NUEVO
    Task<TicketResponseDto> UpdateTicketAsync(Guid ticketId, UpdateTicketDto dto);
    Task<TicketResponseDto> CloseTicketAsync(Guid ticketId); // ✅ NUEVO
    Task<bool> DeleteTicketAsync(Guid ticketId);
    Task<ResponseDto> AddResponseToTicketAsync(CreateResponseDto dto, Guid responderId);
}