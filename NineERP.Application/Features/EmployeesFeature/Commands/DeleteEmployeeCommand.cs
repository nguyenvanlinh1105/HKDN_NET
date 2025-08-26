using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Dat;

namespace NineERP.Application.Features.EmployeesFeature.Commands;

public class DeleteEmployeeCommand(long id) : IRequest<IResult>
{
    public long Id { get; set; } = id;

    public class Handler(
        IApplicationDbContext context,
        ICurrentUserService currentUser
    ) : IRequestHandler<DeleteEmployeeCommand, IResult>
    {
        public async Task<IResult> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            var user = currentUser.UserName ?? "System";

            // Bắt đầu transaction
            await using var transaction = await context.BeginTransactionAsync(cancellationToken);
            try
            {
                var employee = await context.DatEmployees
                    .FirstOrDefaultAsync(e => e.Id == request.Id && !e.IsDeleted, cancellationToken);

                if (employee == null)
                    return await Result.FailAsync("Employee not found.");

                // Tìm AppUser theo Email (hoặc theo mối quan hệ nếu có)
                var appUser = await context.Users
                    .FirstOrDefaultAsync(u => u.Email == employee.Email && !u.IsDeleted, cancellationToken);

                // Xoá mềm Employee
                employee.IsDeleted = true;
                employee.LastModifiedBy = user;
                employee.LastModifiedOn = DateTime.UtcNow;
                context.DatEmployees.Update(employee);

                // Xoá mềm AppUser nếu có
                if (appUser != null)
                {
                    appUser.IsDeleted = true;
                    appUser.LastModifiedBy = user;
                    appUser.LastModifiedOn = DateTime.UtcNow;
                    context.Users.Update(appUser);
                }

                await context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return await Result.SuccessAsync("Employee and login account deleted successfully.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                return await Result.FailAsync($"Delete failed: {ex.Message}");
            }
        }
    }
}
