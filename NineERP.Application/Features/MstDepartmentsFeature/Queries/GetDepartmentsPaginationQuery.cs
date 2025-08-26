using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.MstDepartment;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using System.Linq.Dynamic.Core;

namespace NineERP.Application.Features.MstDepartmentsFeature.Queries;

public record GetDepartmentsPaginationQuery(DepartmentRequest Request) : IRequest<PaginatedResult<DepartmentDetailDto>>;

public class GetDepartmentsPaginationQueryHandler : IRequestHandler<GetDepartmentsPaginationQuery, PaginatedResult<DepartmentDetailDto>>
{
    private readonly IApplicationDbContext _context;

    public GetDepartmentsPaginationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<PaginatedResult<DepartmentDetailDto>> Handle(GetDepartmentsPaginationQuery request, CancellationToken cancellationToken)
    {
        var keyword = request.Request.Keyword?.ToLower();

        var query = _context.MstDepartments
            .AsNoTracking()
            .Where(x => !x.IsDeleted &&
                (string.IsNullOrEmpty(keyword) ||
                 x.NameVi.ToLower().Contains(keyword) ||
                 x.NameEn.ToLower().Contains(keyword) ||
                 x.NameJa.ToLower().Contains(keyword)));

        var totalRecords = await query.CountAsync(cancellationToken);

        var result = await query
            .OrderBy(request.Request.OrderBy)
            .Skip((request.Request.PageNumber - 1) * request.Request.PageSize)
            .Take(request.Request.PageSize)
            .Select(x => new DepartmentDetailDto
            {
                Id = x.Id,
                NameVi = x.NameVi,
                NameEn = x.NameEn,
                NameJa = x.NameJa,
                ParentId = x.ParentId,
                Description = x.Description
            })
            .ToListAsync(cancellationToken);

        return PaginatedResult<DepartmentDetailDto>.Success(result, totalRecords, request.Request.PageNumber, request.Request.PageSize);
    }
}
