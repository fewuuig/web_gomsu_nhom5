using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using QuanLyBanDoGomSu.Application.DTOs;
using QuanLyBanDoGomSu.Application.Port.In;
using QuanLyBanDoGomSu.Exception;

namespace QuanLyBanDoGomSu.Presentation.Controllers.API
{
    [ApiController]
    [Route("api/v1/chat")]
    public class ChatApiController : ControllerBase
    {
        private readonly IChatService _chatService;

        public ChatApiController(IChatService chatService)
        {
            _chatService = chatService;
        }

        /// <summary>
        /// Lấy toàn bộ lịch sử tin nhắn tư vấn đồ gốm sứ của 1 Session Chat
        /// GET /api/v1/chat/messages?sessionId=SESSION_123
        /// </summary>
        [HttpGet("messages")]
        public async Task<IActionResult> GetMessages([FromQuery] string sessionId)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new ValidationCustomException("Vui lòng cung cấp mã phiên chat (sessionId)");
            }

            var messages = await _chatService.GetMessagesAsync(sessionId);
            var response = ApiResponse<List<ChatMessageDto>>.SuccessResponse(
                messages, "Lấy lịch sử tin nhắn tư vấn thành công");

            return Ok(response);
        }

        /// <summary>
        /// Lấy lịch sử tin nhắn phân trang theo con trỏ Cursor (khi trượt lên để tải thêm tin nhắn cũ)
        /// GET /api/v1/chat/messages/cursor?sessionId=SESSION_123&cursorId=100&limit=20
        /// </summary>
        [HttpGet("messages/cursor")]
        public async Task<IActionResult> GetMessagesCursor(
            [FromQuery] string sessionId,
            [FromQuery] string? cursorId = null,
            [FromQuery] int limit = 20)
        {
            if (string.IsNullOrWhiteSpace(sessionId))
            {
                throw new ValidationCustomException("Vui lòng cung cấp mã phiên chat (sessionId)");
            }

            var pagedResult = await _chatService.GetMessagesCursorAsync(sessionId, cursorId, limit);
            var response = ApiResponse<CursorPagedResponse<ChatMessageDto>>.SuccessResponse(
                pagedResult, "Lấy danh sách tin nhắn phân trang con trỏ thành công");

            return Ok(response);
        }

        /// <summary>
        /// Gửi tin nhắn tư vấn trực tuyến (Khách hàng <-> Tư vấn viên Admin)
        /// POST /api/v1/chat/send
        /// Quy tắc Append-Only: Chỉ cho phép Tạo (Send) và Đọc (Read), không cho sửa hay xóa tin nhắn
        /// </summary>
        [HttpPost("send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageRequest request)
        {
            if (!ModelState.IsValid)
            {
                throw new ValidationCustomException("Nội dung tin nhắn không hợp lệ");
            }

            var sent = await _chatService.SendMessageAsync(request);
            var response = ApiResponse<ChatMessageDto>.SuccessResponse(
                sent, "Gửi tin nhắn tư vấn thành công");

            return Ok(response);
        }

        /// <summary>
        /// Lấy danh sách tất cả các phiên chat tư vấn khách hàng (Dành cho Quản trị viên/Nhân viên hỗ trợ)
        /// GET /api/v1/chat/sessions
        /// </summary>
        [HttpGet("sessions")]
        public async Task<IActionResult> GetSessions()
        {
            var sessions = await _chatService.GetSessionsAsync();
            var response = ApiResponse<List<ChatSessionDto>>.SuccessResponse(
                sessions, "Lấy danh sách các phiên tư vấn khách hàng thành công");

            return Ok(response);
        }
    }
}
