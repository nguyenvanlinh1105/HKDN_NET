using Microsoft.EntityFrameworkCore;
using NineERP.Application.Interfaces.Persistence;

namespace NineERP.Infrastructure.Services.Identity
{
    public class TokenBlacklistService(IApplicationDbContext dbContext)
    {
        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
            return await dbContext.RevokedTokens.AnyAsync(t => t.Token == token);
        }
    }

}
