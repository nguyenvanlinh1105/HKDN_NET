using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.DatCustomer;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.DatCustomerFeature.Queries;
public record GetDatCustomerByIdQuery(int Id) : IRequest<IResult<DatCustomerDto>>;
public class GetDatCustomerByIdQueryHandler : IRequestHandler<GetDatCustomerByIdQuery, IResult<DatCustomerDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IMapper _mapper;
    public GetDatCustomerByIdQueryHandler(IApplicationDbContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }
    public async Task<IResult<DatCustomerDto>> Handle(GetDatCustomerByIdQuery request, CancellationToken cancellationToken)
    {
        var entity = await _context.DatCustomers.AsNoTracking().FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);
        if (entity == null) return await Result<DatCustomerDto>.FailAsync(MessageConstants.NotFound);

        var dto = _mapper.Map<DatCustomerDto>(entity);
        return await Result<DatCustomerDto>.SuccessAsync(dto);
    }
}