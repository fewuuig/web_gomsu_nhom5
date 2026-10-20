using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using QuanLyBanDoGomSu.Application.Port.In;
using QuanLyBanDoGomSu.Application.Port.Out;
using QuanLyBanDoGomSu.Application.Service;
using QuanLyBanDoGomSu.Infrastructure.Cache;
using QuanLyBanDoGomSu.Infrastructure.Persistence;
using QuanLyBanDoGomSu.Infrastructure.Persistence.Repositories;
using QuanLyBanDoGomSu.Presentation.Hubs;

var builder = WebApplication.CreateBuilder(args);

// 1. Cấu hình DbContext kết nối SQL Server 2022 local
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<QuanLyBanDoGomSuDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. Đăng ký Dependency Injection cho Repositories (Output Ports)
builder.Services.AddScoped<IDanhMucHangHoaRepository, DanhMucHangHoaRepository>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IStatisticsRepository, StatisticsRepository>();

// 3. Đăng ký Dependency Injection cho Services (Input Ports)
builder.Services.AddScoped<IDanhMucHangHoaService, DanhMucHangHoaService>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IStatisticsService, StatisticsService>();
builder.Services.AddScoped<IEmailService, QuanLyBanDoGomSu.Infrastructure.Services.EmailService>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IAddressService, AddressService>();

// 4. Đăng ký Redis Cache Seam (Tiền tố key: 'GomSuNhom5:')
bool isRedisEnabled = builder.Configuration.GetValue<bool>("RedisConfig:Enabled");
if (isRedisEnabled)
{
    builder.Services.AddSingleton<ICacheService, RedisCacheService>();
}
else
{
    builder.Services.AddSingleton<ICacheService, InMemoryCacheService>();
}

// 5. Cấu hình SignalR WebSocket cho Chat tư vấn thời gian thực
builder.Services.AddSignalR();

// 6. Cấu hình CORS kết nối với Frontend React (d:\Fe_gomsu_nhom5)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:5173", "http://localhost:5174")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddControllers(options =>
{
    options.Filters.Add<QuanLyBanDoGomSu.Exception.GlobalExceptionHandlerFilter>();
})
.ConfigureApiBehaviorOptions(options =>
{
    options.InvalidModelStateResponseFactory = context =>
    {
        var errors = context.ModelState.Values
            .SelectMany(v => v.Errors)
            .Select(e => e.ErrorMessage)
            .ToList();
        string errorDetails = string.Join("; ", errors);
        var response = QuanLyBanDoGomSu.Exception.ApiResponse<object>.ErrorResponse(
            400, "Thông tin dữ liệu gửi lên không hợp lệ!", errorDetails);
        return new BadRequestObjectResult(response);
    };
});

var app = builder.Build();

// 7. Tự động kiểm tra, tạo CSDL và nạp dữ liệu mẫu + tài khoản test khi ứng dụng khởi chạy
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<QuanLyBanDoGomSuDbContext>();
        DatabaseInitializer.Initialize(context);
    }
    catch (System.Exception ex)
    {
        var logger = services.GetService<Microsoft.Extensions.Logging.ILogger<Program>>();
        logger?.LogWarning(ex, "Lưu ý khi tự động kết nối/khởi tạo CSDL: {Message}", ex.Message);
    }
}

app.UseCors("AllowReactApp");
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/hubs/chat");

app.Run();
