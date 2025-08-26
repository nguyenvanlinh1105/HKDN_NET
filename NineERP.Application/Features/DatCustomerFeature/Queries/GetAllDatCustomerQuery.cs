using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.DatCustomer;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.DatCustomerFeature.Queries;
public record GetAllDatCustomerQuery : IRequest<IResult<List<DatCustomerDto>>>;
public class GetAllDatCustomerQueryHandler : IRequestHandler<GetAllDatCustomerQuery, IResult<List<DatCustomerDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetAllDatCustomerQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IResult<List<DatCustomerDto>>> Handle(GetAllDatCustomerQuery request, CancellationToken cancellationToken)
    {
        var list = await _context.DatCustomers.AsNoTracking().Where(x => !x.IsDeleted).ToListAsync(cancellationToken);
        var result = _mapper.Map<List<DatCustomerDto>>(list);
        return await Result<List<DatCustomerDto>>.SuccessAsync(result);
    }
}
