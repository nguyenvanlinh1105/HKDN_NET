using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.KbnContractType;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.KbnContractTypeFeature.Queries;
public record GetKbnContractTypeByIdQuery(int Id) : IRequest<IResult<KbnContractTypeDto>>;
public class GetKbnContractTypeByIdQueryHandler : IRequestHandler<GetKbnContractTypeByIdQuery, IResult<KbnContractTypeDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetKbnContractTypeByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IResult<KbnContractTypeDto>> Handle(GetKbnContractTypeByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.KbnContractTypes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        if (entity == null) return await Result<KbnContractTypeDto>.FailAsync(MessageConstants.NotFound);

        var dto = _mapper.Map<KbnContractTypeDto>(entity);
        return await Result<KbnContractTypeDto>.SuccessAsync(dto);
    }
}