using AutoMapper;
using MediatR;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.KbnLeaveType;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Kbn;

namespace NineERP.Application.Features.KbnLeaveTypeFeature.Commands;
public record CreateKbnLeaveTypeCommand(KbnLeaveTypeDto KbnLeaveType) : IRequest<IResult>
{
    public class CreateKbnLeaveTypeCommandHandler(
        IApplicationDbContext context,
        IMapper mapper) : IRequestHandler<CreateKbnLeaveTypeCommand, IResult>
    {
        public async Task<IResult> Handle(CreateKbnLeaveTypeCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await context.BeginTransactionAsync(cancellationToken);
            try
            {
                // 1. Tạo KbnLeaveType
                var kbnLeaveType = mapper.Map<KbnLeaveType>(request.KbnLeaveType);
                kbnLeaveType.CreatedOn = DateTime.Now;
                context.KbnLeaveTypes.Add(kbnLeaveType);

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
