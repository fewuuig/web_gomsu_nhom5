using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace QuanLyBanDoGomSu.Application.DTOs
{
    public class SendMessageRequest
    {
        [Required]
        public string SessionId { get; set; }

        public string SenderId { get; set; } = string.Empty;

        public string SenderName { get; set; } = "Khách Hàng";

        [Required]
        public string Content { get; set; }

        public bool IsNhanVien { get; set; } = false;
    }

    public class ChatMessageDto
    {
        public string Id { get; set; }
        public string SessionId { get; set; }
        public string SenderId { get; set; }
        public string SenderName { get; set; }
        public string Content { get; set; }
        public bool IsNhanVien { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class CursorPagedResponse<T>
    {
        public List<T> Items { get; set; } = new();
        public string? NextCursor { get; set; }
        public bool HasMore { get; set; }
    }

    public class ChatSessionDto
    {
        public string SessionId { get; set; } = string.Empty;
        public string CustomerId { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string LastMessage { get; set; } = string.Empty;
        public DateTime LastTimestamp { get; set; }
        public int TotalMessages { get; set; }
        public bool HasUnreadByAdmin { get; set; }
    }
}
