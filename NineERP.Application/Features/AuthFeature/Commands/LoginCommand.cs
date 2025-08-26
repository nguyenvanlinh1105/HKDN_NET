using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.Auth;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Identity;

namespace NineERP.Application.Features.AuthFeature.Commands
{
    public class LoginCommand : IRequest<GenericResponse<AuthResponse>>
    {
        public string Username { get; set; } = default!;
        public string Password { get; set; } = default!;
        public bool RememberMe { get; set; }

        public class Handler(IConfiguration configuration, UserManager<AppUser> userManager, ILogger<LoginCommand> log) : IRequestHandler<LoginCommand, GenericResponse<AuthResponse>>
        {
            public async Task<GenericResponse<AuthResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
            {
                var user = await userManager.FindByNameAsync(request.Username);

                if (user == null || !await userManager.CheckPasswordAsync(user, request.Password))
                {
                    log.LogWarning("Login failed for user: {User}", request.Username);
                    return GenericResponse<AuthResponse>.ErrorResponse(401, ErrorMessages.GetMessage("AUTH000"), "AUTH001", ErrorMessages.GetMessage("AUTH001"));
                }

                if (user.LockoutEnabled)
                {
                    log.LogWarning("Login blocked for locked user: {User}", request.Username);
                    return GenericResponse<AuthResponse>.ErrorResponse(403, ErrorMessages.GetMessage("AUTH002"), "AUTH003", ErrorMessages.GetMessage("AUTH003"));
                }

                var jwtSettings = configuration.GetSection("JwtSettings");

                var accessToken = GenerateAccessToken(user.UserName!);
                var refreshToken = GenerateRefreshToken();

                var refreshTokenExpiry = DateTime.UtcNow.AddDays(Convert.ToDouble(jwtSettings["RefreshTokenExpirationInDays"]));
                if (request.RememberMe) refreshTokenExpiry =  refreshTokenExpiry.AddDays(15);

                // 🔵 Save Refresh Token to Identity
                await userManager.SetAuthenticationTokenAsync(user, "JWT", "RefreshToken", refreshToken);
                await userManager.SetAuthenticationTokenAsync(user, "JWT", "AccessToken", accessToken);
                await userManager.SetAuthenticationTokenAsync(user, "JWT", "RefreshTokenExpiry", refreshTokenExpiry.ToString("o"));

                return GenericResponse<AuthResponse>.SuccessResponse(200, string.Empty, new AuthResponse { AccessToken = accessToken, RefreshToken = refreshToken});
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
