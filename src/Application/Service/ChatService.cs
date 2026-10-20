using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using QuanLyBanDoGomSu.Application.DTOs;
using QuanLyBanDoGomSu.Application.Port.In;
using QuanLyBanDoGomSu.Domain.Entities;
using QuanLyBanDoGomSu.Exception;
using QuanLyBanDoGomSu.Infrastructure.Persistence;
using QuanLyBanDoGomSu.Presentation.Hubs;

namespace QuanLyBanDoGomSu.Application.Service
{
    public class ChatService : IChatService
    {
        private readonly QuanLyBanDoGomSuDbContext _context;
        private readonly IHubContext<ChatHub> _hubContext;

        public ChatService(QuanLyBanDoGomSuDbContext context, IHubContext<ChatHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task<ChatMessageDto> SendMessageAsync(SendMessageRequest request)
        {
            // Nghiệp vụ 1: Ngăn chặn tài khoản Quản trị viên/Nhân viên tự gửi tin nhắn tư vấn đóng vai khách hàng
            if (!request.IsNhanVien)
            {
                // Bắt buộc khách hàng phải đăng nhập tài khoản (MaQuyen = 'KH') mới được gửi tin nhắn tư vấn
                if (string.IsNullOrWhiteSpace(request.SenderId))
                {
                    throw new ValidationCustomException("Vui lòng đăng nhập tài khoản khách hàng để gửi tin nhắn tư vấn gốm sứ.");
                }

                var senderAccount = await _context.TaiKhoans
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.MaTK == request.SenderId || t.TenDangNhap == request.SenderId);

                if (senderAccount == null)
                {
                    throw new ValidationCustomException("Tài khoản không tồn tại. Vui lòng đăng nhập để tiếp tục.");
                }

                if (senderAccount.MaQuyen != "KH")
                {
                    throw new ValidationCustomException("Tài khoản Quản trị viên/Nhân viên không được phép gửi tin nhắn tự tư vấn cho chính mình.");
                }

                if (!string.IsNullOrWhiteSpace(request.SenderName))
                {
                    var lowerName = request.SenderName.Trim().ToLower();
                    if (lowerName == "admin" || lowerName == "quản trị viên" || lowerName == "quan tri vien" || lowerName.StartsWith("admin_"))
                    {
                        throw new ValidationCustomException("Tên người gửi không hợp lệ. Quản trị viên không được phép gửi tin tư vấn đóng vai khách hàng.");
                    }
                }
            }
            // Nghiệp vụ 2: Khi phản hồi với vai trò Nhân viên/Admin (IsNhanVien = true)
            else
            {
                if (string.IsNullOrWhiteSpace(request.SenderId))
                {
                    throw new ValidationCustomException("Vui lòng cung cấp mã định danh Quản trị viên/Nhân viên hợp lệ.");
                }

                // Chặn nếu tài khoản gửi không tồn tại hoặc là tài khoản khách hàng (mạo danh Admin)
                var senderAccount = await _context.TaiKhoans
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.MaTK == request.SenderId || t.TenDangNhap == request.SenderId);

                if (senderAccount == null || senderAccount.MaQuyen == "KH")
                {
                    throw new ValidationCustomException("Chỉ tài khoản Quản trị viên/Nhân viên hợp lệ mới được phép gửi tin nhắn với tư cách nhân viên tư vấn.");
                }

                // Chặn tuyệt đối Admin tự phản hồi vào phiên tư vấn của chính mình
                if (request.SessionId.Equals($"SESS_KH_{request.SenderId}", StringComparison.OrdinalIgnoreCase)
                    || request.SessionId.Equals(request.SenderId, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ValidationCustomException("Quản trị viên không thể chat tư vấn cho chính tài khoản của mình.");
                }

                // Chặn nếu phiên này được tạo bởi chính người gửi
                var existingCustomerMsg = await _context.TinNhanChats
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.SessionId == request.SessionId && !t.IsNhanVien);

                if (existingCustomerMsg != null && !string.IsNullOrWhiteSpace(existingCustomerMsg.SenderId)
                    && existingCustomerMsg.SenderId.Equals(request.SenderId, StringComparison.OrdinalIgnoreCase))
                {
                    throw new ValidationCustomException("Quản trị viên không thể gửi tin nhắn tư vấn cho chính mình.");
                }
            }

            var entity = new TinNhanChat
            {
                SessionId = request.SessionId,
                SenderId = string.IsNullOrWhiteSpace(request.SenderId) ? Guid.NewGuid().ToString("N") : request.SenderId,
                SenderName = string.IsNullOrWhiteSpace(request.SenderName) ? "Khách Hàng" : request.SenderName,
                Content = request.Content,
                IsNhanVien = request.IsNhanVien,
                ThoiGian = DateTime.Now
            };

            _context.TinNhanChats.Add(entity);
            await _context.SaveChangesAsync();

            var dto = new ChatMessageDto
            {
                Id = entity.Id.ToString(),
                SessionId = entity.SessionId,
                SenderId = entity.SenderId,
                SenderName = entity.SenderName,
                Content = entity.Content,
                IsNhanVien = entity.IsNhanVien,
                Timestamp = entity.ThoiGian
            };

            // Broadcast qua SignalR WebSocket Hub cho các client đang lướt phiên chat
            try
            {
                await _hubContext.Clients.Group(request.SessionId).SendAsync("ReceiveMessage", dto);

                // Xác định tên định danh thực tế của khách hàng trong phiên này
                string resolvedCustName = !dto.IsNhanVien ? dto.SenderName : "Khách Hàng";
                string resolvedCustId = !dto.IsNhanVien ? dto.SenderId : string.Empty;

                if (dto.IsNhanVien)
                {
                    // Khi Admin/Nhân viên gửi tin nhắn, tìm tin nhắn gần nhất của khách để lấy đúng tên khách
                    var custMsg = await _context.TinNhanChats
                        .AsNoTracking()
                        .Where(t => t.SessionId == request.SessionId && !t.IsNhanVien)
                        .OrderByDescending(t => t.Id)
                        .FirstOrDefaultAsync();

                    if (custMsg != null)
                    {
                        resolvedCustId = custMsg.SenderId;
                        resolvedCustName = custMsg.SenderName;
                    }

                    // Nếu mã phiên theo tài khoản SESS_KH_{maTK}, tra cứu trực tiếp KhachHang/TaiKhoan
                    if (request.SessionId.StartsWith("SESS_KH_", StringComparison.OrdinalIgnoreCase))
                    {
                        var maTK = request.SessionId.Substring("SESS_KH_".Length);
                        var tk = await _context.TaiKhoans
                            .AsNoTracking()
                            .Include(t => t.KhachHang)
                            .FirstOrDefaultAsync(t => t.MaTK == maTK);

                        if (tk?.KhachHang != null && !string.IsNullOrWhiteSpace(tk.KhachHang.TenKhach))
                        {
                            resolvedCustName = tk.KhachHang.TenKhach;
                        }
                        else if (tk != null && !string.IsNullOrWhiteSpace(tk.TenDangNhap))
                        {
                            resolvedCustName = tk.TenDangNhap;
                        }
                    }
                }

                // Broadcast thông báo cập nhật phiên chat tới nhóm Admin
                await _hubContext.Clients.Group("Admins").SendAsync("SessionUpdated", new ChatSessionDto
                {
                    SessionId = dto.SessionId,
                    CustomerId = resolvedCustId,
                    CustomerName = resolvedCustName,
                    LastMessage = dto.Content,
                    LastTimestamp = dto.Timestamp,
                    HasUnreadByAdmin = !dto.IsNhanVien
                });
            }
            catch
            {
                // Soft handle if no client is listening
            }

            return dto;
        }

        public async Task<List<ChatMessageDto>> GetMessagesAsync(string sessionId)
        {
            var entities = await _context.TinNhanChats
                .AsNoTracking()
                .Where(t => t.SessionId == sessionId)
                .OrderBy(t => t.Id)
                .ToListAsync();

            return entities.Select(e => new ChatMessageDto
            {
                Id = e.Id.ToString(),
                SessionId = e.SessionId,
                SenderId = e.SenderId,
                SenderName = e.SenderName,
                Content = e.Content,
                IsNhanVien = e.IsNhanVien,
                Timestamp = e.ThoiGian
            }).ToList();
        }

        /// <summary>
        /// Phân trang tin nhắn theo con trỏ Cursor (trượt lên để tải thêm tin nhắn cũ hơn)
        /// Tối ưu hiệu năng index B-Tree SQL Server O(1)
        /// </summary>
        public async Task<CursorPagedResponse<ChatMessageDto>> GetMessagesCursorAsync(string sessionId, string? cursorId, int limit = 20)
        {
            var query = _context.TinNhanChats
                .AsNoTracking()
                .Where(t => t.SessionId == sessionId);

            if (!string.IsNullOrWhiteSpace(cursorId) && long.TryParse(cursorId, out long cId))
            {
                query = query.Where(t => t.Id < cId);
            }

            // Lấy limit + 1 phần tử để xác định xem còn tin nhắn cũ hơn nữa hay không (HasMore)
            var rawList = await query
                .OrderByDescending(t => t.Id)
                .Take(limit + 1)
                .ToListAsync();

            bool hasMore = rawList.Count > limit;
            var fetched = rawList.Take(limit).ToList();

            string? nextCursor = fetched.Count > 0 ? fetched.Last().Id.ToString() : null;

            // Đảo ngược thứ tự để trả về dạng Chronological (Tăng dần theo Id) để Frontend hiển thị từ trên xuống
            var chronologicallyOrdered = fetched.OrderBy(t => t.Id).Select(e => new ChatMessageDto
            {
                Id = e.Id.ToString(),
                SessionId = e.SessionId,
                SenderId = e.SenderId,
                SenderName = e.SenderName,
                Content = e.Content,
                IsNhanVien = e.IsNhanVien,
                Timestamp = e.ThoiGian
            }).ToList();

            return new CursorPagedResponse<ChatMessageDto>
            {
                Items = chronologicallyOrdered,
                NextCursor = nextCursor,
                HasMore = hasMore
            };
        }

        /// <summary>
        /// Lấy danh sách tất cả các phiên tư vấn khách hàng (Dành riêng cho Admin Dashboard)
        /// Phân loại và lấy tin nhắn gần nhất cùng thông tin khách hàng
        /// </summary>
        public async Task<List<ChatSessionDto>> GetSessionsAsync()
        {
            var allMessages = await _context.TinNhanChats
                .AsNoTracking()
                .OrderByDescending(t => t.Id)
                .ToListAsync();

            // Lấy danh sách map tài khoản khách hàng để map tên thật TenKhach chính xác 100%
            var accountMap = await _context.TaiKhoans
                .AsNoTracking()
                .Include(t => t.KhachHang)
                .ToDictionaryAsync(
                    t => t.MaTK,
                    t => t.KhachHang != null && !string.IsNullOrWhiteSpace(t.KhachHang.TenKhach)
                        ? t.KhachHang.TenKhach
                        : (!string.IsNullOrWhiteSpace(t.TenDangNhap) ? t.TenDangNhap : "Khách Hàng")
                );

            var sessions = allMessages
                .GroupBy(t => t.SessionId)
                .Select(g =>
                {
                    var latest = g.First();
                    // Lấy tin nhắn khách hàng gần đây nhất để lấy định danh mới nhất
                    var latestCustomerMsg = g.FirstOrDefault(m => !m.IsNhanVien) ?? latest;

                    string customerName = latestCustomerMsg.SenderName;
                    string customerId = latestCustomerMsg.SenderId;

                    // Nếu là phiên tài khoản SESS_KH_{maTK}, ưu tiên lấy họ tên thật trong CSDL
                    if (g.Key.StartsWith("SESS_KH_", StringComparison.OrdinalIgnoreCase))
                    {
                        var maTK = g.Key.Substring("SESS_KH_".Length);
                        if (accountMap.TryGetValue(maTK, out var realName))
                        {
                            customerName = realName;
                            customerId = maTK;
                        }
                    }
                    else if (!string.IsNullOrWhiteSpace(customerId) && accountMap.TryGetValue(customerId, out var realName2))
                    {
                        customerName = realName2;
                    }

                    if (string.IsNullOrWhiteSpace(customerName))
                    {
                        customerName = "Khách Hàng";
                    }

                    return new ChatSessionDto
                    {
                        SessionId = g.Key,
                        CustomerId = customerId,
                        CustomerName = customerName,
                        LastMessage = latest.Content,
                        LastTimestamp = latest.ThoiGian,
                        TotalMessages = g.Count(),
                        HasUnreadByAdmin = !latest.IsNhanVien
                    };
                })
                .OrderByDescending(s => s.LastTimestamp)
                .ToList();

            return sessions;
        }
    }
}
