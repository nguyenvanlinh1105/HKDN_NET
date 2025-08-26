using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.KbnDeviceType;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.KbnDeviceTypeFeature.Queries;
public record GetKbnDeviceTypeByIdQuery(int Id) : IRequest<IResult<KbnDeviceTypeDto>>;
public class GetKbnDeviceTypeByIdQueryHandler : IRequestHandler<GetKbnDeviceTypeByIdQuery, IResult<KbnDeviceTypeDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetKbnDeviceTypeByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IResult<KbnDeviceTypeDto>> Handle(GetKbnDeviceTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.KbnDeviceTypes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        if (entity == null) return await Result<KbnDeviceTypeDto>.FailAsync(MessageConstants.NotFound);

        var dto = _mapper.Map<KbnDeviceTypeDto>(entity);
        return await Result<KbnDeviceTypeDto>.SuccessAsync(dto);
    }
}