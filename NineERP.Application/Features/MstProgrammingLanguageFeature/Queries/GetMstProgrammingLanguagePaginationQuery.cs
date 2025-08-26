using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.MstProgrammingLanguage;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using System.Linq.Dynamic.Core;

namespace NineERP.Application.Features.MstProgrammingLanguageFeature.Queries;
public record GetMstProgrammingLanguagePaginationQuery(MstProgrammingLanguageRequest Request) : IRequest<PaginatedResult<MstProgrammingLanguageDto>>;
public class GetMstProgrammingLanguagePaginationQueryHandler : IRequestHandler<GetMstProgrammingLanguagePaginationQuery, PaginatedResult<MstProgrammingLanguageDto>>
{
    private readonly IApplicationDbContext _context;
    public GetMstProgrammingLanguagePaginationQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }
    public async Task<PaginatedResult<MstProgrammingLanguageDto>> Handle(
        GetMstProgrammingLanguagePaginationQuery request,
        CancellationToken cancellationToken)
    {
        var query = _context.MstProgrammingLanguages.AsNoTracking()
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
            .Select(x => new MstProgrammingLanguageDto
            {
                Id = x.Id,
                NameVi = x.NameVi,
                NameEn = x.NameEn,
                NameJp = x.NameJp,
                Description = x.Description,
            })
            .ToListAsync(cancellationToken);

        return PaginatedResult<MstProgrammingLanguageDto>.Success(result, totalRecords, request.Request.PageNumber, request.Request.PageSize);
    }
}
