using AutoMapper;
using MediatR;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.KbnHolidayType;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Kbn;

namespace NineERP.Application.Features.KbnHolidayTypeFeature.Commands;
public record CreateKbnHolidayTypeCommand(KbnHolidayTypeDto KbnHolidayType) : IRequest<IResult>
{
    public class CreateKbnHolidayTypeCommandHandler(
        IApplicationDbContext context,
        IMapper mapper) : IRequestHandler<CreateKbnHolidayTypeCommand, IResult>
    {
        public async Task<IResult> Handle(CreateKbnHolidayTypeCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await context.BeginTransactionAsync(cancellationToken);
            try
            {
                // 1. Tạo KbnHolidayType
                var kbnHolidayType = mapper.Map<KbnHolidayType>(request.KbnHolidayType);
                kbnHolidayType.CreatedOn = DateTime.Now;
                context.KbnHolidayTypes.Add(kbnHolidayType);

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
