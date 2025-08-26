using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.MstLocation;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.MstLocationFeature.Queries;
public record GetProvincesByCountryIdQuery(short CountryId) : IRequest<IResult<List<MstProvincesDto>>>;
public class GetProvincesByCountryIdQueryHandler : IRequestHandler<GetProvincesByCountryIdQuery, IResult<List<MstProvincesDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetProvincesByCountryIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IResult<List<MstProvincesDto>>> Handle(GetProvincesByCountryIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.MstProvinces.AsNoTracking().Where(x => x.CountryId == request.CountryId && !x.IsDeleted).ToListAsync(cancellationToken);

        var dto = _mapper.Map<List<MstProvincesDto>>(entity);
        return await Result<List<MstProvincesDto>>.SuccessAsync(dto);
    }
}
