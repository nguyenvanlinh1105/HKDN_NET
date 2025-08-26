using MediatR;
using NineERP.Application.Constants.Employee;
using NineERP.Application.Dtos.DatCustomer;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.DatCustomerFeature.Queries;
public record GetStatusQuery : IRequest<IResult<List<CustomerStatusDto>>>;
public class GetStatusQueryHandler : IRequestHandler<GetStatusQuery, IResult<List<CustomerStatusDto>>>
{
    public async Task<IResult<List<CustomerStatusDto>>> Handle(GetStatusQuery request, CancellationToken cancellationToken)
    {
        var list = Enum.GetValues(typeof(StaticVariable.CustomerStatus))
            .Cast<StaticVariable.CustomerStatus>()
            .Select(e => new CustomerStatusDto
            {
                Id = (int)e,
                Name = e.ToString()
            }).ToList();

        return await Result<List<CustomerStatusDto>>.SuccessAsync(list);
    }
}