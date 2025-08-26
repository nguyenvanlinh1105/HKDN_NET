using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.MstShift;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Mst;

namespace NineERP.Application.Features.MstShiftFeature.Commands;
public record CreateMstShiftCommand(MstShiftDto MstShift) : IRequest<IResult>
{
    public class CreateDeviceTypeCommandHandler(
        IApplicationDbContext context,
        IMapper mapper) : IRequestHandler<CreateMstShiftCommand, IResult>
    {
        public async Task<IResult> Handle(CreateMstShiftCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await context.BeginTransactionAsync(cancellationToken);
            try
            {
                if (request.MstShift.IsDefault)
                {
                    var shiftDefault = await context.MstShifts.FirstOrDefaultAsync(x => x.IsDefault == true && !x.IsDeleted);
                    if (shiftDefault != null)
                    {
                        shiftDefault.IsDefault = false;
                    }
                }
                // 1. Tạo MstShift
                var mstShift = mapper.Map<MstShift>(request.MstShift);
                mstShift.CreatedOn = DateTime.Now;
                context.MstShifts.Add(mstShift);

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
