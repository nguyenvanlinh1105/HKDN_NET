using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.User;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using System.Linq.Dynamic.Core;

namespace NineERP.Application.Features.UsersFeature.Queries;

public record GetUsersPaginationQuery(UserRequest Request) : IRequest<PaginatedResult<UserResponse>>
{
    public class Handler(IApplicationDbContext context)
        : IRequestHandler<GetUsersPaginationQuery, PaginatedResult<UserResponse>>
    {
        public async Task<PaginatedResult<UserResponse>> Handle(GetUsersPaginationQuery request, CancellationToken cancellationToken)
        {
            var query = from u in context.Users.AsNoTracking().Where(x => !x.IsDeleted && !x.UserName!.Equals("superadmin"))
                        join ur in context.UserRoles.AsNoTracking() on u.Id equals ur.UserId
                        join r in context.Roles.Where(x => string.IsNullOrEmpty(request.Request.RoleName) || x.Name == request.Request.RoleName)
                            on ur.RoleId equals r.Id
                        where string.IsNullOrEmpty(request.Request.Keyword)
                              || u.FullName.ToLower().Contains(request.Request.Keyword.ToLower())
                              || u.Email!.ToLower().Contains(request.Request.Keyword.ToLower())
                        select new UserResponse
                        {
                            Id = u.Id,
                            Email = u.Email,
                            FullName = u.FullName,
                            CreatedOn = u.CreatedOn,
                            AvatarUrl = u.AvatarUrl,
                            LockoutEnabled = u.LockoutEnabled,
                            RoleName = r.Name
                        };

            var totalRecords = await query.CountAsync(cancellationToken);

            var result = await query.OrderBy(request.Request.OrderBy)
                                    .Skip((request.Request.PageNumber - 1) * request.Request.PageSize)
                                    .Take(request.Request.PageSize)
                                    .ToListAsync(cancellationToken);

            return PaginatedResult<UserResponse>.Success(result, totalRecords, request.Request.PageNumber, request.Request.PageSize);
        }
    }
}
