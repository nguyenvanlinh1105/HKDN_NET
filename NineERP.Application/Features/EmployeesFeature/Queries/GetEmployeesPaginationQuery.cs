using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.Employees;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using System.Globalization;
using System.Linq.Dynamic.Core;

namespace NineERP.Application.Features.EmployeesFeature.Queries;

public record GetEmployeesPaginationQuery(EmployeeRequest Request) : IRequest<PaginatedResult<EmployeeResponse>>
{
    public class Handler(IApplicationDbContext context)
        : IRequestHandler<GetEmployeesPaginationQuery, PaginatedResult<EmployeeResponse>>
    {
        public async Task<PaginatedResult<EmployeeResponse>> Handle(GetEmployeesPaginationQuery request, CancellationToken cancellationToken)
        {
            var keyword = request.Request.Keyword?.Trim().ToLower();

            // ✅ Lấy ngôn ngữ hiện tại (vi, en, ja)
            var culture = CultureInfo.CurrentUICulture.Name.ToLower();
            bool isVi = culture.StartsWith("vi");
            bool isJa = culture.StartsWith("ja");

            var query = from emp in context.DatEmployees.AsNoTracking()
                        where !emp.IsDeleted &&
                              (string.IsNullOrEmpty(keyword) ||
                               emp.FullName.ToLower().Contains(keyword) ||
                               emp.Email.ToLower().Contains(keyword))

                        join user in context.Users on emp.EmployeeNo equals user.UserName into empUsers
                        from user in empUsers.DefaultIfEmpty()

                        join position in context.MstPositions on emp.PositionId equals position.Id into positions
                        from position in positions.DefaultIfEmpty()

                        join dept in context.MstDepartments on emp.DepartmentId equals dept.Id into depts
                        from dept in depts.DefaultIfEmpty()

                        join contractType in context.KbnContractTypes on emp.ContractTypeId equals contractType.Id into contractTypes
                        from contractType in contractTypes.DefaultIfEmpty()

                        join status in context.KbnEmployeeStatus on emp.EmployeeStatusId equals status.Id into statuses
                        from status in statuses.DefaultIfEmpty()

                        select new EmployeeResponse
                        {
                            Id = emp.Id,
                            EmployeeNo = emp.EmployeeNo,
                            FullName = emp.FullName,
                            Email = emp.Email,
                            PhoneNo = emp.PhoneNo,
                            CreatedOn = emp.CreatedOn,
                            AvatarUrl = user.AvatarUrl,
                            Status = isVi ? status.NameVi :
                                     isJa ? status.NameJp :
                                     status.NameEn,
                            PositionName = isVi ? position.NameVi :
                                           isJa ? position.NameJa :
                                           position.NameEn,
                            DepartmentName = isVi ? dept.NameVi :
                                             isJa ? dept.NameJa :
                                             dept.NameEn,
                            ContractTypeName = isVi ? contractType.NameVi :
                                               isJa ? contractType.NameJp :
                                               contractType.NameEn
                        };

            var totalRecords = await query.CountAsync(cancellationToken);

            var orderBy = !string.IsNullOrWhiteSpace(request.Request.OrderBy)
                ? request.Request.OrderBy
                : "CreatedOn desc";

            var data = await query
                .OrderBy(orderBy)
                .Skip((request.Request.PageNumber - 1) * request.Request.PageSize)
                .Take(request.Request.PageSize)
                .ToListAsync(cancellationToken);

            return PaginatedResult<EmployeeResponse>.Success(data, totalRecords, request.Request.PageNumber, request.Request.PageSize);
        }
    }
}
