using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.KbnContractType;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.KbnContractTypeFeature.Queries;
public record GetAllKbnContractTypesQuery : IRequest<IResult<List<KbnContractTypeDto>>>;
public class GetAllContractTypesQueryHandler : IRequestHandler<GetAllKbnContractTypesQuery, IResult<List<KbnContractTypeDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetAllContractTypesQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IResult<List<KbnContractTypeDto>>> Handle(GetAllKbnContractTypesQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.KbnContractTypes.AsNoTracking().Where(x => !x.IsDeleted).ToListAsync(cancellationToken);
        var result = _mapper.Map<List<KbnContractTypeDto>>(list);
        return await Result<List<KbnContractTypeDto>>.SuccessAsync(result);
    }
}
