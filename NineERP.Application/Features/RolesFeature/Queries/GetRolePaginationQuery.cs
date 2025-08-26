using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.Role;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using System.Linq.Dynamic.Core;

namespace NineERP.Application.Features.RolesFeature.Queries;

public class GetRolePaginationQuery(RoleRequest request) : IRequest<PaginatedResult<RoleResponse>>
{
    public RoleRequest Request { get; set; } = request;

    public class Handler(IApplicationDbContext context) : IRequestHandler<GetRolePaginationQuery, PaginatedResult<RoleResponse>>
    {
        public async Task<PaginatedResult<RoleResponse>> Handle(GetRolePaginationQuery query, CancellationToken cancellationToken)
        {
            var request = query.Request;

            var roleQuery = context.Roles.AsNoTracking().Where(x => !x.IsDeleted);

            if (!string.IsNullOrEmpty(request.Keyword))
            {
                roleQuery = roleQuery.Where(r => r.Name!.ToLower().Contains(request.Keyword.ToLower()));
            }

            var totalRecord = await roleQuery.CountAsync(cancellationToken);

            var result = await roleQuery.OrderBy(request.OrderBy)
                                        .Skip((request.PageNumber - 1) * request.PageSize)
                                        .Take(request.PageSize)
                                        .Select(r => new RoleResponse
                                        {
                                            Id = r.Id,
                                            Name = r.Name!,
                                            Description = r.Description,
                                            CreatedBy = r.CreatedBy,
                                            CreatedOn = r.CreatedOn
                                        })
                                        .ToListAsync(cancellationToken);

            return PaginatedResult<RoleResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }
    }
}