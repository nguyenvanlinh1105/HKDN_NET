using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.MstShift;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.MstShiftFeature.Commands;
public record UpdateMstShiftCommand(MstShiftDto MstShift) : IRequest<IResult>
{
    public class Handler(IApplicationDbContext context, IMapper mapper)
        : IRequestHandler<UpdateMstShiftCommand, IResult>
    {
        public async Task<IResult> Handle(UpdateMstShiftCommand request, CancellationToken cancellationToken)
        {
            var mstShift = await context.MstShifts
                .FirstOrDefaultAsync(x => x.Id == request.MstShift.Id && !x.IsDeleted, cancellationToken);

            if (mstShift == null)
                return await Result.FailAsync(MessageConstants.NotFound);

            mstShift = mapper.Map(request.MstShift, mstShift);
            mstShift.LastModifiedOn = DateTime.Now;
            context.MstShifts.Update(mstShift);
            await context.SaveChangesAsync(cancellationToken);

            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
        }
    }
}

