using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NineERP.Infrastructure.Contexts;

namespace NineERP.Infrastructure.Services.Identity
{
    public class TokenCleanupService(IServiceScopeFactory scopeFactory, ILogger<TokenCleanupService> logger) : BackgroundService
    {
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = TimeSpan.FromHours(1);
                logger.LogInformation($"🕛 Token cleanup sẽ chạy vào {delay} UTC.");

                try
                {
                    // ⏳ Chờ đến đúng thời gian 24h UTC
                    await Task.Delay(delay, stoppingToken);

                    // Kiểm tra hủy bỏ trước khi thực hiện công việc chính
                    stoppingToken.ThrowIfCancellationRequested();

                    // Tiến hành thực hiện công việc khi thời gian đến
                    using var scope = scopeFactory.CreateScope();
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                    var expirationThreshold = DateTime.UtcNow.AddDays(-1); // 📅 Xoá token đã thu hồi 1 ngày trước
                    var expiredTokens = await dbContext.RevokedTokens
                        .Where(t => t.RevokedAt < expirationThreshold)
                        .ToListAsync(stoppingToken);

                    if (expiredTokens.Any())
                    {
                        dbContext.RevokedTokens.RemoveRange(expiredTokens);
                        await dbContext.SaveChangesAsync(stoppingToken);
                        logger.LogInformation($"🗑️ Xóa {expiredTokens.Count} revoked tokens cũ.");
                    }
                }
                catch (TaskCanceledException)
                {
                    // Xử lý nếu tác vụ bị hủy
                    logger.LogInformation("❌ Task đã bị hủy bỏ.");
                    break; // Dừng vòng lặp nếu tác vụ bị hủy
                }
                catch (Exception ex)
                {
                    logger.LogError($"❌ Lỗi khi cleanup revoked tokens: {ex.Message}");
                }
            }
        }
    }

}
