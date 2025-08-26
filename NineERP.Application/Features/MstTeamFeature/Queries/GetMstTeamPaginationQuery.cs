using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.MstTeam;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using System.Linq.Dynamic.Core;

namespace NineERP.Application.Features.MstTeamFeature.Queries;
public record GetMstTeamPaginationQuery(MstTeamRequest Request) : IRequest<PaginatedResult<MstTeamDto>>;
public class GetMstTeamPaginationQueryHandler : IRequestHandler<GetMstTeamPaginationQuery, PaginatedResult<MstTeamDto>>
{
    private readonly IApplicationDbContext _context;
    public GetMstTeamPaginationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<PaginatedResult<MstTeamDto>> Handle(
        GetMstTeamPaginationQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.MstTeams.AsNoTracking()
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
            .Select(x => new MstTeamDto
            {
                Id = x.Id,
                NameVi = x.NameVi,
                NameEn = x.NameEn,
                NameJp = x.NameJp,
                Description = x.Description,
            })
            .ToListAsync(cancellationToken);

        return PaginatedResult<MstTeamDto>.Success(result, totalRecords, request.Request.PageNumber, request.Request.PageSize);
    }
}
