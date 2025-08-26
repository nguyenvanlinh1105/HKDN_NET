using MediatR;
using Microsoft.AspNetCore.Identity;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.Role;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Identity;

namespace NineERP.Application.Features.RolesFeature.Commands;

public class AddRoleCommand(RoleDetailDto dto) : IRequest<IResult>
{
    public RoleDetailDto Dto { get; set; } = dto;

    public class Handler(IApplicationDbContext context, RoleManager<AppRole> roleManager) : IRequestHandler<AddRoleCommand, IResult>
    {
        public async Task<IResult> Handle(AddRoleCommand request, CancellationToken cancellationToken)
        {
            var role = new AppRole(request.Dto.Name, request.Dto.Description ?? string.Empty);
            await roleManager.CreateAsync(role);
            await context.SaveChangesAsync(cancellationToken);
            return await Result.SuccessAsync(MessageConstants.AddSuccess);
        }
    }
}