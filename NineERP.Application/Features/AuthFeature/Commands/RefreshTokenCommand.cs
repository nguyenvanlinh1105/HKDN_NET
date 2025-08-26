using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using NineERP.Application.Dtos.Auth;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Identity;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Interfaces.Persistence;
using System.Security.Cryptography;
using NineERP.Application.Constants.Messages;

namespace NineERP.Application.Features.AuthFeature.Commands
{
    public class RefreshTokenCommand : IRequest<GenericResponse<AuthResponse>>
    {
        public string RefreshToken { get; set; } = default!;

        public class Handler(IConfiguration configuration, UserManager<AppUser> userManager, IApplicationDbContext context)
            : IRequestHandler<RefreshTokenCommand, GenericResponse<AuthResponse>>
        {
            public async Task<GenericResponse<AuthResponse>> Handle(RefreshTokenCommand request,
                                                                    CancellationToken cancellationToken)
            {
                var refreshToken = await context.UserTokens.AsNoTracking().FirstOrDefaultAsync(t => t.LoginProvider == "JWT" && t.Name == "RefreshToken" && t.Value == request.RefreshToken, cancellationToken);

                if (refreshToken == null)
                {
                    return GenericResponse<AuthResponse>.ErrorResponse(401, ErrorMessages.GetMessage("AUTH009"), "AUTH009",
                        ErrorMessages.GetMessage("AUTH009"));
                }

                var user = await userManager.FindByIdAsync(refreshToken.UserId);
                if (user == null) {
                    return GenericResponse<AuthResponse>.ErrorResponse(401, ErrorMessages.GetMessage("AUTH009"), "AUTH009",
                        ErrorMessages.GetMessage("AUTH009"));
                }

                var expiryTokenString = await userManager.GetAuthenticationTokenAsync(user, "JWT", "RefreshTokenExpiry");

                // 🔥 Check expiration time
                if (!DateTime.TryParse(expiryTokenString, out var expiryDate) || expiryDate < DateTime.UtcNow)
                {
                    return GenericResponse<AuthResponse>.ErrorResponse(401, ErrorMessages.GetMessage("AUTH010"), "AUTH010",
                        ErrorMessages.GetMessage("AUTH010"));
                }

                var jwtSettings = configuration.GetSection("JwtSettings");

                var newAccessToken = GenerateAccessToken(user.UserName!);

                // 🔄 Update new Refresh Token
                var newRefreshToken = GenerateRefreshToken();
                var newExpiryDate =
                    DateTime.UtcNow.AddDays(Convert.ToDouble(jwtSettings["RefreshTokenExpirationInDays"]));

                await userManager.RemoveAuthenticationTokenAsync(user, "JWT", "RefreshToken");
                await userManager.RemoveAuthenticationTokenAsync(user, "JWT", "AccessToken");
                await userManager.RemoveAuthenticationTokenAsync(user, "JWT", "RefreshTokenExpiry");
                await userManager.SetAuthenticationTokenAsync(user, "JWT", "RefreshToken", newRefreshToken);
                await userManager.SetAuthenticationTokenAsync(user, "JWT", "AccessToken", newAccessToken);
                await userManager.SetAuthenticationTokenAsync(user, "JWT", "RefreshTokenExpiry",
                    newExpiryDate.ToString("o"));

                return GenericResponse<AuthResponse>.SuccessResponse(200, "Token refreshed", new AuthResponse
                {
                    AccessToken = newAccessToken,
                    RefreshToken = newRefreshToken
                });
            }

            private string GenerateAccessToken(string username)
            {
                var jwtSettings = configuration.GetSection("JwtSettings");
                var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!));

                var tokenDescriptor = new SecurityTokenDescriptor
                {
                    Subject = new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, username) }),
                    Expires = DateTime.UtcNow.AddMinutes(Convert.ToDouble(jwtSettings["AccessTokenExpirationInMinutes"])),
                    Issuer = jwtSettings["Issuer"],
                    Audience = jwtSettings["Audience"],
                    SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256)
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                return tokenHandler.WriteToken(tokenHandler.CreateToken(tokenDescriptor));
            }

            private string GenerateRefreshToken()
            {
                var randomNumber = new byte[32];
                using var rng = RandomNumberGenerator.Create();
                rng.GetBytes(randomNumber);
                return Convert.ToBase64String(randomNumber);
            }
        }
    }
}
