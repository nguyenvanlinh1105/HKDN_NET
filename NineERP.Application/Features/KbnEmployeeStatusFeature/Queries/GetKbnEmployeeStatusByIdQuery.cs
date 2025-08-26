using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.KbnEmployeeStatus;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.KbnEmployeeStatusFeature.Queries;
public record GetKbnEmployeeStatusByIdQuery(int Id) : IRequest<IResult<KbnEmployeeStatusDto>>;
public class GetKbnEmployeeStatusByIdQueryHandler : IRequestHandler<GetKbnEmployeeStatusByIdQuery, IResult<KbnEmployeeStatusDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetKbnEmployeeStatusByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IResult<KbnEmployeeStatusDto>> Handle(GetKbnEmployeeStatusByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.KbnEmployeeStatus.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        if (entity == null) return await Result<KbnEmployeeStatusDto>.FailAsync(MessageConstants.NotFound);

        var dto = _mapper.Map<KbnEmployeeStatusDto>(entity);
        return await Result<KbnEmployeeStatusDto>.SuccessAsync(dto);
    }
}