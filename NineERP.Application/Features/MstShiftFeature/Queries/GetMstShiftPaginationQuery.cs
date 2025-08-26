using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.MstShift;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using System.Linq.Dynamic.Core;

namespace NineERP.Application.Features.MstShiftFeature.Queries;
public record GetMstShiftPaginationQuery(MstShiftRequest Request) : IRequest<PaginatedResult<MstShiftDto>>;
public class GetMstShiftPaginationQueryHandler : IRequestHandler<GetMstShiftPaginationQuery, PaginatedResult<MstShiftDto>>
{
    private readonly IApplicationDbContext _context;
    public GetMstShiftPaginationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<PaginatedResult<MstShiftDto>> Handle(
        GetMstShiftPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.MstShifts.AsNoTracking()
            .Where(x => !x.IsDeleted &&
                (string.IsNullOrEmpty(request.Request.Keyword) ||
                x.MorningStartTime.ToLower().Contains(request.Request.Keyword.ToLower())));

        var totalRecords = await query.CountAsync(cancellationToken);

        var result = await query
            .OrderBy(request.Request.OrderBy)
            .Skip((request.Request.PageNumber - 1) * request.Request.PageSize)
            .Take(request.Request.PageSize)
            .Select(x => new MstShiftDto
            {
                Id = x.Id,
                MorningStartTime = x.MorningStartTime,
                MorningEndTime = x.MorningEndTime,
                AfternoonStartTime = x.AfternoonStartTime,
                AfternoonEndTime = x.AfternoonEndTime,
                IsDefault = x.IsDefault,
                TotalHour = x.TotalHour,
                Description = x.Description,
                NumberOfEmployee = _context.DatEmployeeShifts
                .Count(es => es.ShiftId == x.Id && !es.IsDeleted
                             && _context.DatEmployees.Any(emp => emp.EmployeeNo == es.EmployeeNo && !emp.IsDeleted))
            })
            .ToListAsync(cancellationToken);

        return PaginatedResult<MstShiftDto>.Success(result, totalRecords, request.Request.PageNumber, request.Request.PageSize);
    }
}
