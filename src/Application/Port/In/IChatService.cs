using System.Collections.Generic;
using System.Threading.Tasks;
using QuanLyBanDoGomSu.Application.DTOs;

namespace QuanLyBanDoGomSu.Application.Port.In
{
    public interface IChatService
    {
        Task<ChatMessageDto> SendMessageAsync(SendMessageRequest request);
        Task<List<ChatMessageDto>> GetMessagesAsync(string sessionId);
        Task<CursorPagedResponse<ChatMessageDto>> GetMessagesCursorAsync(string sessionId, string? cursorId, int limit = 20);
        Task<List<ChatSessionDto>> GetSessionsAsync();
    }
}
