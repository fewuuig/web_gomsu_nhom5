using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using QuanLyBanDoGomSu.Application.DTOs;
using QuanLyBanDoGomSu.Application.Port.In;

namespace QuanLyBanDoGomSu.Presentation.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IChatService _chatService;

        public ChatHub(IChatService chatService)
        {
            _chatService = chatService;
        }

        /// <summary>
        /// Tham gia nhóm phiên tư vấn trực tuyến (Session ID)
        /// </summary>
        public async Task JoinSession(string sessionId)
        {
            if (!string.IsNullOrWhiteSpace(sessionId))
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, sessionId);
            }
        }

        /// <summary>
        /// Rời khỏi nhóm phiên tư vấn
        /// </summary>
        public async Task LeaveSession(string sessionId)
        {
            if (!string.IsNullOrWhiteSpace(sessionId))
            {
                await Groups.RemoveFromGroupAsync(Context.ConnectionId, sessionId);
            }
        }

        /// <summary>
        /// Admin/Nhân viên tham gia nhóm Admins để nhận thông báo real-time khi có tin nhắn từ bất kỳ phiên nào
        /// </summary>
        public async Task JoinAdminGroup()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "Admins");
        }

        public async Task LeaveAdminGroup()
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "Admins");
        }

        /// <summary>
        /// Gửi tin nhắn tư vấn gốm sứ thời gian thực qua WebSocket Hub
        /// Lưu trực tiếp vào CSDL SQL Server và phát sóng tới tất cả thành viên trong nhóm session
        /// </summary>
        public async Task SendMessage(SendMessageRequest request)
        {
            var messageDto = await _chatService.SendMessageAsync(request);

            // Broadcast cho tất cả client thuộc cùng SessionId
            await Clients.Group(request.SessionId).SendAsync("ReceiveMessage", messageDto);
        }
    }
}
