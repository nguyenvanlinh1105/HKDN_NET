using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.MstTeam;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.MstTeamFeature.Queries;
public record GetMstTeamByIdQuery(int Id) : IRequest<IResult<MstTeamDto>>;
public class GetMstTeamByIdQueryHandler : IRequestHandler<GetMstTeamByIdQuery, IResult<MstTeamDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetMstTeamByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IResult<MstTeamDto>> Handle(GetMstTeamByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.MstTeams.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        if (entity == null) return await Result<MstTeamDto>.FailAsync(MessageConstants.NotFound);

        var dto = _mapper.Map<MstTeamDto>(entity);
        return await Result<MstTeamDto>.SuccessAsync(dto);
    }
}