using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Employee;
using NineERP.Application.Dtos.KbnEmployeeStatus;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using System.Linq.Dynamic.Core;

namespace NineERP.Application.Features.KbnEmployeeStatusFeature.Queries;
public record GetKbnEmployeeStatusPaginationQuery(KbnEmployeeStatusRequest Request) : IRequest<PaginatedResult<KbnEmployeeStatusDto>>;
public class GetKbnEmployeeStatusPaginationQueryHandler(IApplicationDbContext context)
    : IRequestHandler<GetKbnEmployeeStatusPaginationQuery, PaginatedResult<KbnEmployeeStatusDto>>
{
    public async Task<PaginatedResult<KbnEmployeeStatusDto>> Handle(
        GetKbnEmployeeStatusPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var query = context.KbnEmployeeStatus.AsNoTracking()
            .Where(x => !x.IsDeleted &&
                (string.IsNullOrEmpty(request.Request.Keyword) ||
                x.NameVi.ToLower().Contains(request.Request.Keyword.ToLower())
                || x.NameEn.ToLower().Contains(request.Request.Keyword.ToLower()) 
                || x.NameJp.ToLower().Contains(request.Request.Keyword.ToLower())));

        var totalRecords = await query.CountAsync(cancellationToken);

        var result = await query
            .OrderBy(request.Request.OrderBy)
            .Skip((request.Request.PageNumber - 1) * request.Request.PageSize)
            .Take(request.Request.PageSize)
            .Select(x => new KbnEmployeeStatusDto
            {
                Id = x.Id,
                NameVi = x.NameVi,
                NameEn = x.NameEn,
                NameJp = x.NameJp,
                Description = x.Description,
            })
            .ToListAsync(cancellationToken);

        return PaginatedResult<KbnEmployeeStatusDto>.Success(result, totalRecords, request.Request.PageNumber, request.Request.PageSize);
    }
}
