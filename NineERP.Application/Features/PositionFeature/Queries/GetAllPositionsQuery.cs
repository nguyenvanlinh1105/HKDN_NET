using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.MstPosition;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.PositionFeature.Queries;

public record GetAllPositionsQuery : IRequest<IResult<List<PositionDetailDto>>>;

public class GetAllPositionsQueryHandler : IRequestHandler<GetAllPositionsQuery, IResult<List<PositionDetailDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;

    public GetAllPositionsQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<IResult<List<PositionDetailDto>>> Handle(GetAllPositionsQuery request, CancellationToken cancellationToken)
    {
        var positions = await _context.MstPositions
            .AsNoTracking()
            .Where(x => !x.IsDeleted)
            .ToListAsync(cancellationToken);

        var result = _mapper.Map<List<PositionDetailDto>>(positions);
        return await Result<List<PositionDetailDto>>.SuccessAsync(result);
    }
}
