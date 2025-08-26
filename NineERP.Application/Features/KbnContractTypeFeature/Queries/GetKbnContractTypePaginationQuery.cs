using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.KbnContractType;
using NineERP.Application.Dtos.KbnLeaveType;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using System.Linq.Dynamic.Core;

namespace NineERP.Application.Features.KbnContractTypeFeature.Queries;
public record GetKbnContractTypePaginationQuery(KbnContractTypeRequest Request) : IRequest<PaginatedResult<KbnContractTypeDto>>;
public class GetKbnContractTypePaginationQueryHandler : IRequestHandler<GetKbnContractTypePaginationQuery, PaginatedResult<KbnContractTypeDto>>
{
    private readonly IApplicationDbContext _context;
    public GetKbnContractTypePaginationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<PaginatedResult<KbnContractTypeDto>> Handle(
        GetKbnContractTypePaginationQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.KbnContractTypes.AsNoTracking()
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
            .Select(x => new KbnContractTypeDto
            {
                Id = x.Id,
                NameVi = x.NameVi,
                NameEn = x.NameEn,
                NameJp = x.NameJp,
                Description = x.Description,
            })
            .ToListAsync(cancellationToken);

        return PaginatedResult<KbnContractTypeDto>.Success(result, totalRecords, request.Request.PageNumber, request.Request.PageSize);
    }
}
