using MediatR;
using NineERP.Application.Constants.Employee;
using NineERP.Application.Dtos.KbnLeaveType;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.KbnLeaveTypeFeature.Queries;
public record GetLeaveTypeFlagQuery : IRequest<IResult<List<LeaveTypeFlagDto>>>;
public class GetLeaveTypeFlagQueryHandler : IRequestHandler<GetLeaveTypeFlagQuery, IResult<List<LeaveTypeFlagDto>>>
{
    public async Task<IResult<List<LeaveTypeFlagDto>>> Handle(GetLeaveTypeFlagQuery request, CancellationToken cancellationToken)
    {
        var list = Enum.GetValues(typeof(StaticVariable.MyLeaveType))
            .Cast<StaticVariable.MyLeaveType>()
            .Select(e => new LeaveTypeFlagDto
            {
                Id = (int)e,
                Name = e.ToString()
            }).ToList();

        return await Result<List<LeaveTypeFlagDto>>.SuccessAsync(list);
    }
}