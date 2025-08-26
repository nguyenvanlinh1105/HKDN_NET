using MediatR;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using System.Security.Claims;

namespace NineERP.Application.Features.RolesFeature.Commands;

public class UpdateRoleClaimsCommand(string roleId, List<Claim> claims) : IRequest<IResult>
{
    public string RoleId { get; set; } = roleId;
    public List<Claim> Claims { get; set; } = claims;

    public class Handler(IApplicationDbContext dbContext) : IRequestHandler<UpdateRoleClaimsCommand, IResult>
    {
        public async Task<IResult> Handle(UpdateRoleClaimsCommand request, CancellationToken cancellationToken)
        {
            var existingClaims = dbContext.RoleClaims.Where(rc => rc.RoleId == request.RoleId);
            dbContext.RoleClaims.RemoveRange(existingClaims);

            var newClaims = request.Claims.Select(c => new Domain.Entities.Identity.AppRoleClaim
            {
                RoleId = request.RoleId,
                ClaimType = c.Type,
                ClaimValue = c.Value
            });

            await dbContext.RoleClaims.AddRangeAsync(newClaims, cancellationToken);
            await dbContext.SaveChangesAsync(cancellationToken);

            return await Result.SuccessAsync("Cập nhật quyền thành công");
        }
    }
}
