using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.Role;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Identity;

namespace NineERP.Application.Features.RolesFeature.Commands;

public class UpdateRoleCommand(RoleDetailDto dto) : IRequest<IResult>
{
    public RoleDetailDto Dto { get; set; } = dto;

    public class Handler(IApplicationDbContext context, RoleManager<AppRole> roleManager) : IRequestHandler<UpdateRoleCommand, IResult>
    {
        public async Task<IResult> Handle(UpdateRoleCommand request, CancellationToken cancellationToken)
        {
            var role = await context.Roles.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == request.Dto.Id, cancellationToken);

            if (role == null) return await Result.FailAsync(MessageConstants.NotFound);

            role.Name = request.Dto.Name;
            role.Description = request.Dto.Description;

            await roleManager.UpdateAsync(role);
            await context.SaveChangesAsync(cancellationToken);

            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
        }
    }
}
