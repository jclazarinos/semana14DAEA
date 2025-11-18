using Lab10_Lazarinos.Application.DTOs.Tickets;

namespace Lab10_Lazarinos.Application.Interfaces.Services;

public interface IResponseService
{
    Task<ResponseDto> GetResponseByIdAsync(Guid responseId);
    Task<IEnumerable<ResponseDto>> GetResponsesByTicketIdAsync(Guid ticketId);
    Task<ResponseDto> UpdateResponseAsync(Guid responseId, string newMessage, Guid userId);
    Task<bool> DeleteResponseAsync(Guid responseId, Guid userId);
}