using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.Employees;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.EmployeesFeature.Commands;

public record UpdateEmployeeGeneralCommand(EmployeeGeneralDto EmployeeGeneral) : IRequest<GenericResponse<object>>
{
    public class Handler(IApplicationDbContext context, IMapper mapper, ICurrentUserService currentUserService) : IRequestHandler<UpdateEmployeeGeneralCommand, GenericResponse<object>>
    {
        public async Task<GenericResponse<object>> Handle(UpdateEmployeeGeneralCommand request, CancellationToken cancellationToken)
        {
            var employee = await context.DatEmployees.FirstOrDefaultAsync(x => !x.IsDeleted && x.EmployeeNo == currentUserService.EmployeeNo, cancellationToken);
            var user = await context.Users.FirstOrDefaultAsync(x => x.UserName == currentUserService.EmployeeNo, cancellationToken);
            
            if (employee == null || user == null)
                return GenericResponse<object>.ErrorResponse(400, ErrorMessages.GetMessage("E001"), "E001",
                    ErrorMessages.GetMessage("E001"));

            mapper.Map(request.EmployeeGeneral, employee);
            context.DatEmployees.Update(employee);

            user.FullName = request.EmployeeGeneral.FullName;
            user.PhoneNumber = request.EmployeeGeneral.PhoneNo;
            context.Users.Update(user);

            await context.SaveChangesAsync(cancellationToken);

            return GenericResponse<object>.SuccessResponse(200, ErrorMessages.GetMessage("SYS0001"), "");
        }
    }
}
