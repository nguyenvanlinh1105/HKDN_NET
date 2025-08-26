using AutoMapper;
using MediatR;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.KbnEmployeeStatus;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Kbn;

namespace NineERP.Application.Features.KbnEmployeeStatusFeature.Commands;
public record CreateKbnEmployeeStatusCommand(KbnEmployeeStatusDto KbnEmployeeStatus) : IRequest<IResult>
{
    public class CreateKbnEmployeeStatusCommandHandler(
        IApplicationDbContext context,
        IMapper mapper) : IRequestHandler<CreateKbnEmployeeStatusCommand, IResult>
    {
        public async Task<IResult> Handle(CreateKbnEmployeeStatusCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await context.BeginTransactionAsync(cancellationToken);
            try
            {
                // 1. Tạo KbnEmployeeStatus
                var kbnEmployeeStatus = mapper.Map<KbnEmployeeStatus>(request.KbnEmployeeStatus);
                kbnEmployeeStatus.CreatedOn = DateTime.Now;
                context.KbnEmployeeStatus.Add(kbnEmployeeStatus);

                // 2. Lưu vào database
                await context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return await Result.SuccessAsync(MessageConstants.AddSuccess);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                return await Result.FailAsync($"Create failed: {ex.Message}");
            }
        }
    }
}
