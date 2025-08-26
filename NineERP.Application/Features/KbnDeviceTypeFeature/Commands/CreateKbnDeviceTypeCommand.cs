using AutoMapper;
using MediatR;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.KbnDeviceType;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Kbn;

namespace NineERP.Application.Features.KbnDeviceTypeFeature.Commands;
public record CreateKbnDeviceTypeCommand(KbnDeviceTypeDto kbnDeviceType) : IRequest<IResult>
{
    public class CreateDeviceTypeCommandHandler(
        IApplicationDbContext context,
        IMapper mapper) : IRequestHandler<CreateKbnDeviceTypeCommand, IResult>
    {
        public async Task<IResult> Handle(CreateKbnDeviceTypeCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await context.BeginTransactionAsync(cancellationToken);
            try
            {
                // 1. Tạo KbnDeviceType
                var deviceType = mapper.Map<KbnDeviceType>(request.kbnDeviceType);
                deviceType.CreatedOn = DateTime.Now;
                context.KbnDeviceTypes.Add(deviceType);

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
