using MediatR;
using NineERP.Application.Wrapper;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Domain.Entities.Identity;

namespace NineERP.Application.Features.AuthFeature.Commands
{
    public class LogoutCommand : IRequest<GenericResponse<object>>
    {
        public string UserName { get; set; } = default!;

        public class Handler(IApplicationDbContext context) : IRequestHandler<LogoutCommand, GenericResponse<object>>
        {
            public async Task<GenericResponse<object>> Handle(LogoutCommand request, CancellationToken cancellationToken)
            {
                var idUser = await context.Users.AsNoTracking().Where(x => x.UserName == request.UserName).Select(y => y.Id).FirstOrDefaultAsync(cancellationToken);
                var userTokens = await context.UserTokens.Where(t => t.LoginProvider == "JWT" && t.UserId == idUser).ToListAsync(cancellationToken);
                if (userTokens.Count < 0)
                {
                    return GenericResponse<object>.ErrorResponse(401, ErrorMessages.GetMessage("AUTH009"), "AUTH009",
                        ErrorMessages.GetMessage("AUTH009"));
                }

                await using var transaction = await context.BeginTransactionAsync(cancellationToken);
                try
                {
                    // 🔥 Delete Refresh Token
                    context.UserTokens.RemoveRange(userTokens);
                    var revokedTokens = userTokens.Where(x => x.Name == "AccessToken").Select(x => new RevokedToken
                    {
                        Token = x.Value!,
                        RevokedAt = DateTime.UtcNow
                    }).ToList();

                    // Save recovery token (Revoke accessToken)
                    await context.RevokedTokens.AddRangeAsync(revokedTokens, cancellationToken);
                    await context.SaveChangesAsync(cancellationToken);

                    await transaction.CommitAsync(cancellationToken);
                }
                catch (Exception)
                {
                    await transaction.RollbackAsync(cancellationToken);

                    return GenericResponse<object>.ErrorResponse(500, ErrorMessages.GetMessage("AUTH011"), "AUTH011", ErrorMessages.GetMessage("AUTH011"));
                }

                return GenericResponse<object>.SuccessResponse(200, "Logged out successfully", null!);
            }
        }
    }
}
