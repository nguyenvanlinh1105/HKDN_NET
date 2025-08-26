using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.KbnLeaveType;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.KbnLeaveTypeFeature.Queries;
public record GetAllKbnLeaveTypeQuery : IRequest<IResult<List<KbnLeaveTypeDto>>>;
public class GetAllKbnLeaveTypeQueryHandler : IRequestHandler<GetAllKbnLeaveTypeQuery, IResult<List<KbnLeaveTypeDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetAllKbnLeaveTypeQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IResult<List<KbnLeaveTypeDto>>> Handle(GetAllKbnLeaveTypeQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.KbnLeaveTypes.AsNoTracking().Where(x => !x.IsDeleted).ToListAsync(cancellationToken);
        var result = _mapper.Map<List<KbnLeaveTypeDto>>(list);
        return await Result<List<KbnLeaveTypeDto>>.SuccessAsync(result);
    }
}
