using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.Role;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.RolesFeature.Queries;

public class GetRoleByIdQuery(string id) : IRequest<Result<RoleDetailDto>>
{
    public string Id { get; set; } = id;

    public class Handler(IApplicationDbContext context) : IRequestHandler<GetRoleByIdQuery, Result<RoleDetailDto>>
    {
        public async Task<Result<RoleDetailDto>> Handle(GetRoleByIdQuery request, CancellationToken cancellationToken)
        {
            var role = await context.Roles.AsNoTracking()
                .FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == request.Id, cancellationToken);

            var result = new RoleDetailDto
            {
                Id = role?.Id ?? string.Empty,
                Name = role?.Name ?? string.Empty,
                Description = role?.Description
            };

            return await Result<RoleDetailDto>.SuccessAsync(result);
        }
    }
}
