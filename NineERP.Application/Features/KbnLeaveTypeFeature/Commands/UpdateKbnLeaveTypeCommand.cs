using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.KbnLeaveType;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.KbnLeaveTypeFeature.Commands;
public record UpdateKbnLeaveTypeCommand(KbnLeaveTypeDto KbnLeaveType) : IRequest<IResult>
{
    public class Handler(IApplicationDbContext context, IMapper mapper)
        : IRequestHandler<UpdateKbnLeaveTypeCommand, IResult>
    {
        public async Task<IResult> Handle(UpdateKbnLeaveTypeCommand request, CancellationToken cancellationToken)
        {
            var kbnLeaveType = await context.KbnLeaveTypes
                .FirstOrDefaultAsync(x => x.Id == request.KbnLeaveType.Id && !x.IsDeleted, cancellationToken);

            if (kbnLeaveType == null)
                return await Result.FailAsync(MessageConstants.NotFound);

            kbnLeaveType = mapper.Map(request.KbnLeaveType, kbnLeaveType);
            kbnLeaveType.LastModifiedOn = DateTime.Now;
            context.KbnLeaveTypes.Update(kbnLeaveType);
            await context.SaveChangesAsync(cancellationToken);

            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
        }
    }
}

