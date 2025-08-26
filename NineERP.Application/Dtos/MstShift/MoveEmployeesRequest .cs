using MediatR;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Dtos.MstShift;

public class MoveEmployeesRequest : IRequest<IResult>
{
    public List<string> EmployeeCodes { get; set; } = new();
    public int FromShiftId { get; set; }
    public int ToShiftId { get; set; }
}
