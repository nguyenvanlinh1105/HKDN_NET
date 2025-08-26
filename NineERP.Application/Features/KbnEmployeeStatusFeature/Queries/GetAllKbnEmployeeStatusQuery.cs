using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.KbnEmployeeStatus;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.KbnEmployeeStatusFeature.Queries;
public record GetAllKbnEmployeeStatusQuery : IRequest<IResult<List<KbnEmployeeStatusDto>>>;
public class GetAllKbnEmployeeStatusQueryHandler : IRequestHandler<GetAllKbnEmployeeStatusQuery, IResult<List<KbnEmployeeStatusDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetAllKbnEmployeeStatusQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IResult<List<KbnEmployeeStatusDto>>> Handle(GetAllKbnEmployeeStatusQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.KbnEmployeeStatus.AsNoTracking().Where(x => !x.IsDeleted).ToListAsync(cancellationToken);
        var result = _mapper.Map<List<KbnEmployeeStatusDto>>(list);
        return await Result<List<KbnEmployeeStatusDto>>.SuccessAsync(result);
    }
}
