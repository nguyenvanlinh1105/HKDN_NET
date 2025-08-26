using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.MstLocation;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.MstLocationFeature.Queries;
public record GetAllMstCountryQuery : IRequest<IResult<List<MstCountryDto>>>;
public class GetAllMstCountryQueryHandler : IRequestHandler<GetAllMstCountryQuery, IResult<List<MstCountryDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetAllMstCountryQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IResult<List<MstCountryDto>>> Handle(GetAllMstCountryQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.MstCountry.AsNoTracking().Where(x => !x.IsDeleted).ToListAsync(cancellationToken);
        var result = _mapper.Map<List<MstCountryDto>>(list);
        return await Result<List<MstCountryDto>>.SuccessAsync(result);
    }
}
