using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.MstTeam;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.MstTeamFeature.Queries;
public record GetAllMstTeamQuery : IRequest<IResult<List<MstTeamDto>>>;
public class GetAllMstTeamQueryHandler : IRequestHandler<GetAllMstTeamQuery, IResult<List<MstTeamDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetAllMstTeamQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IResult<List<MstTeamDto>>> Handle(GetAllMstTeamQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.MstTeams.AsNoTracking().Where(x => !x.IsDeleted).ToListAsync(cancellationToken);
        var result = _mapper.Map<List<MstTeamDto>>(list);
        return await Result<List<MstTeamDto>>.SuccessAsync(result);
    }
}
