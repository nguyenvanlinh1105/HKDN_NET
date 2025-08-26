using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.KbnDeviceType;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.KbnDeviceTypeFeature.Queries;
public record GetAllKbnDeviceTypesQuery : IRequest<IResult<List<KbnDeviceTypeDto>>>;
public class GetAllDeviceTypesQueryHandler : IRequestHandler<GetAllKbnDeviceTypesQuery, IResult<List<KbnDeviceTypeDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetAllDeviceTypesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IResult<List<KbnDeviceTypeDto>>> Handle(GetAllKbnDeviceTypesQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.KbnDeviceTypes.AsNoTracking().Where(x => !x.IsDeleted).ToListAsync(cancellationToken);
        var result = _mapper.Map<List<KbnDeviceTypeDto>>(list);
        return await Result<List<KbnDeviceTypeDto>>.SuccessAsync(result);
    }
}
