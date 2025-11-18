using Lab10_Lazarinos.Application.DTOs.Tickets;
using Lab10_Lazarinos.Application.Interfaces.Persistence;
using Lab10_Lazarinos.Application.Interfaces.Services;

namespace Lab10_Lazarinos.Infrastructure.Services;

public class ResponseService : IResponseService
{
    private readonly IUnitOfWork _unitOfWork;

    public ResponseService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<ResponseDto> GetResponseByIdAsync(Guid responseId)
    {
        var response = await _unitOfWork.Responses.GetByIdAsync(responseId);
        
        if (response == null)
            throw new Exception("Response not found");

        var responder = await _unitOfWork.Users.GetByIdAsync(response.ResponderId);

        return new ResponseDto
        {
            ResponseId = response.ResponseId,
            ResponderId = response.ResponderId,
            ResponderUsername = responder?.Username ?? "Unknown",
            Message = response.Message,
            CreatedAt = response.CreatedAt
        };
    }

    public async Task<IEnumerable<ResponseDto>> GetResponsesByTicketIdAsync(Guid ticketId)
    {
        var responses = await _unitOfWork.Responses.GetResponsesByTicketIdAsync(ticketId);
        
        return responses.Select(r => new ResponseDto
        {
            ResponseId = r.ResponseId,
            ResponderId = r.ResponderId,
            ResponderUsername = r.Responder?.Username ?? "Unknown",
            Message = r.Message,
            CreatedAt = r.CreatedAt
        }).ToList();
    }

    public async Task<ResponseDto> UpdateResponseAsync(Guid responseId, string newMessage, Guid userId)
    {
        var response = await _unitOfWork.Responses.GetByIdAsync(responseId);
        
        if (response == null)
            throw new Exception("Response not found");

        // Solo el autor de la respuesta o un Admin puede editarla
        if (response.ResponderId != userId)
            throw new UnauthorizedAccessException("You can only edit your own responses");

        response.Message = newMessage;

        _unitOfWork.Responses.Update(response);
        await _unitOfWork.SaveChangesAsync();

        return await GetResponseByIdAsync(responseId);
    }

    public async Task<bool> DeleteResponseAsync(Guid responseId, Guid userId)
    {
        var response = await _unitOfWork.Responses.GetByIdAsync(responseId);
        
        if (response == null)
            return false;

        // Solo el autor de la respuesta puede eliminarla
        if (response.ResponderId != userId)
            throw new UnauthorizedAccessException("You can only delete your own responses");

        _unitOfWork.Responses.Remove(response);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }
}