using MediatR;
using NineERP.Application.Constants.Employee;
using NineERP.Application.Dtos.KbnEmployeeStatus;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.KbnEmployeeStatusFeature.Queries;
public record GetStatusQuery : IRequest<IResult<List<EmployeeStatusDto>>>;
public class GetStatusQueryHandler : IRequestHandler<GetStatusQuery, IResult<List<EmployeeStatusDto>>>
{
    public async Task<IResult<List<EmployeeStatusDto>>> Handle(GetStatusQuery request, CancellationToken cancellationToken)
    {
        var list = Enum.GetValues(typeof(StaticVariable.EmployeeStatus))
            .Cast<StaticVariable.EmployeeStatus>()
            .Select(e => new EmployeeStatusDto
            {
                Id = (int)e,
                Name = e.ToString()
            }).ToList();

        return await Result<List<EmployeeStatusDto>>.SuccessAsync(list);
    }
}