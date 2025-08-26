using MediatR;
using NineERP.Application.Constants.Employee;
using NineERP.Application.Dtos.DatCustomer;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.DatCustomerFeature.Queries;
public record GetTypeQuery : IRequest<IResult<List<CustomerTypeDto>>>;
public class GetTypeQueryHandler : IRequestHandler<GetTypeQuery, IResult<List<CustomerTypeDto>>>
{
    public async Task<IResult<List<CustomerTypeDto>>> Handle(GetTypeQuery request, CancellationToken cancellationToken)
    {
        var list = Enum.GetValues(typeof(StaticVariable.CustomerType))
            .Cast<StaticVariable.CustomerType>()
            .Select(e => new CustomerTypeDto
            {
                Id = (int)e,
                Name = e.ToString()
            }).ToList();

        return await Result<List<CustomerTypeDto>>.SuccessAsync(list);
    }
}