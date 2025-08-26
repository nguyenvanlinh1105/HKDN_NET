using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.JsonWebTokens;
using NineERP.Application.Interfaces.Persistence;

namespace NineERP.Web.Authorization
{
    public class BlacklistTokenValidator
    {
        public static async Task ValidateToken(TokenValidatedContext context)
        {
            var scopeFactory = context.HttpContext.RequestServices.GetRequiredService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
            var token = string.Empty;

            if (context.SecurityToken is JsonWebToken jsonWebToken)
            {
                token = jsonWebToken.EncodedToken;
            }
            
            if (string.IsNullOrEmpty(token))
            {
                context.Fail("Invalid token.");
                context.NoResult();
                return;
            }
            
            bool isTokenRevoked = await dbContext.RevokedTokens
                .AsNoTracking()
                .AnyAsync(t => t.Token == token);

            if (isTokenRevoked)
            {
                var logger = scope.ServiceProvider.GetRequiredService<ILogger<BlacklistTokenValidator>>();
                logger.LogWarning($"Access token is blacklisted {token}");
                context.Fail("Token has been revoked. Please log in again.");
                context.NoResult();
            }
        }
    }
}
