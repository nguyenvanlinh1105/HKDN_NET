using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.KbnHolidayType;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.KbnHolidayTypeFeature.Queries;
public record GetAllKbnHolidayTypesQuery : IRequest<IResult<List<KbnHolidayTypeDto>>>;
public class GetAllKbnHolidayTypesQueryHandler : IRequestHandler<GetAllKbnHolidayTypesQuery, IResult<List<KbnHolidayTypeDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetAllKbnHolidayTypesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IResult<List<KbnHolidayTypeDto>>> Handle(GetAllKbnHolidayTypesQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.KbnHolidayTypes.AsNoTracking().Where(x => !x.IsDeleted).ToListAsync(cancellationToken);
        var result = _mapper.Map<List<KbnHolidayTypeDto>>(list);
        return await Result<List<KbnHolidayTypeDto>>.SuccessAsync(result);
    }
}
