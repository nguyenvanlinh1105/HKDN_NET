using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Employee;
using NineERP.Application.Dtos.KbnEmployeeStatus;
using NineERP.Application.Dtos.KbnHolidayType;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using System.Linq.Dynamic.Core;

namespace NineERP.Application.Features.KbnHolidayTypeFeature.Queries;
public record GetKbnHolidayTypesPaginationQuery(KbnHolidayTypeRequest Request) : IRequest<PaginatedResult<KbnHolidayTypeDto>>;
public class GetKbnHolidayTypesPaginationQueryHandler : IRequestHandler<GetKbnHolidayTypesPaginationQuery, PaginatedResult<KbnHolidayTypeDto>>
{
    private readonly IApplicationDbContext _context;
    public GetKbnHolidayTypesPaginationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<PaginatedResult<KbnHolidayTypeDto>> Handle(
        GetKbnHolidayTypesPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.KbnHolidayTypes.AsNoTracking()
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
            .Select(x => new KbnHolidayTypeDto
            {
                Id = x.Id,
                NameVi = x.NameVi,
                NameEn = x.NameEn,
                NameJp = x.NameJp,
                Description = x.Description,
            })
            .ToListAsync(cancellationToken);

        return PaginatedResult<KbnHolidayTypeDto>.Success(result, totalRecords, request.Request.PageNumber, request.Request.PageSize);
    }
}
