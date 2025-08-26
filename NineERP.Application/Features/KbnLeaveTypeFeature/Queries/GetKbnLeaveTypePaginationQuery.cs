using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Employee;
using NineERP.Application.Dtos.KbnEmployeeStatus;
using NineERP.Application.Dtos.KbnLeaveType;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using System.Linq.Dynamic.Core;

namespace NineERP.Application.Features.KbnLeaveTypeFeature.Queries;
public record GetKbnLeaveTypePaginationQuery(KbnLeaveTypeRequest Request) : IRequest<PaginatedResult<KbnLeaveTypeDto>>;
public class GetKbnLeaveTypePaginationQueryHandler : IRequestHandler<GetKbnLeaveTypePaginationQuery, PaginatedResult<KbnLeaveTypeDto>>
{
    private readonly IApplicationDbContext _context;
    public GetKbnLeaveTypePaginationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<PaginatedResult<KbnLeaveTypeDto>> Handle(
        GetKbnLeaveTypePaginationQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.KbnLeaveTypes.AsNoTracking()
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
            .Select(x => new KbnLeaveTypeDto
            {
                Id = x.Id,
                NameVi = x.NameVi,
                NameEn = x.NameEn,
                NameJp = x.NameJp,
                Description = x.Description,
                LeaveTypeFlag = ((StaticVariable.MyLeaveType)x.LeaveTypeFlag).ToString(),
                Acronym = x.Acronym
            })
            .ToListAsync(cancellationToken);

        return PaginatedResult<KbnLeaveTypeDto>.Success(result, totalRecords, request.Request.PageNumber, request.Request.PageSize);
    }
}
