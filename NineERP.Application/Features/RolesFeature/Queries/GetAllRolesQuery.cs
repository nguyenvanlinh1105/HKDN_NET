using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.Role;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using System.Linq.Dynamic.Core;

namespace NineERP.Application.Features.RolesFeature.Queries;

public class GetAllRolesQuery : RoleRequest, IRequest<PaginatedResult<RoleResponse>>
{
    public class Handler(IApplicationDbContext context) : IRequestHandler<GetAllRolesQuery, PaginatedResult<RoleResponse>>
    {
        public async Task<PaginatedResult<RoleResponse>> Handle(GetAllRolesQuery request, CancellationToken cancellationToken)
        {
            var query = from r in context.Roles.AsNoTracking().Where(x => !x.IsDeleted)
                        where string.IsNullOrEmpty(request.Keyword) ||
                              r.Name!.ToLower().Contains(request.Keyword.ToLower())
                        select new RoleResponse
                        {
                            Id = r.Id,
                            Name = r.Name!,
                            Description = r.Description,
                            CreatedOn = r.CreatedOn,
                            CreatedBy = r.CreatedBy
                        };

            var totalRecord = await query.CountAsync(cancellationToken);
            var result = await query.OrderBy(request.OrderBy)
                                    .Skip((request.PageNumber - 1) * request.PageSize)
                                    .Take(request.PageSize)
                                    .ToListAsync(cancellationToken);

            return PaginatedResult<RoleResponse>.Success(result, totalRecord, request.PageNumber, request.PageSize);
        }
    }
}