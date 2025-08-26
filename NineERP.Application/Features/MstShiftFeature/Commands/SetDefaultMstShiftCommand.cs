using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.MstShift;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.MstShiftFeature.Commands;
public record SetDefaultMstShiftCommand(int Id) : IRequest<IResult>
{
    public class Handler(IApplicationDbContext context)
        : IRequestHandler<SetDefaultMstShiftCommand, IResult>
    {
        public async Task<IResult> Handle(SetDefaultMstShiftCommand request, CancellationToken cancellationToken)
        {
            var mstShift = await context.MstShifts
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

            if (mstShift == null) return await Result.FailAsync(MessageConstants.NotFound);
            var shiftDefault = await context.MstShifts.FirstOrDefaultAsync(x => x.IsDefault && !x.IsDeleted, cancellationToken);
            if (shiftDefault != null)
            {
                shiftDefault.IsDefault = false;
                shiftDefault.LastModifiedOn = DateTime.Now;
            }
            mstShift.IsDefault = true;
            mstShift.LastModifiedOn = DateTime.Now;
            context.MstShifts.Update(mstShift);
            await context.SaveChangesAsync(cancellationToken);

            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
        }
    }
}