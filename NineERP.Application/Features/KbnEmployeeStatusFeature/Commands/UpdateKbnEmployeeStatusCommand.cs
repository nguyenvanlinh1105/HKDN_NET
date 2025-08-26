using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.KbnEmployeeStatus;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.KbnEmployeeStatusFeature.Commands;
public record UpdateKbnEmployeeStatusCommand(KbnEmployeeStatusDto KbnEmployeeStatus) : IRequest<IResult>
{
    public class Handler(IApplicationDbContext context, IMapper mapper)
        : IRequestHandler<UpdateKbnEmployeeStatusCommand, IResult>
    {
        public async Task<IResult> Handle(UpdateKbnEmployeeStatusCommand request, CancellationToken cancellationToken)
        {
            var kbnEmployeeStatus = await context.KbnEmployeeStatus
                .FirstOrDefaultAsync(x => x.Id == request.KbnEmployeeStatus.Id && !x.IsDeleted, cancellationToken);

            if (kbnEmployeeStatus == null)
                return await Result.FailAsync(MessageConstants.NotFound);

            kbnEmployeeStatus = mapper.Map(request.KbnEmployeeStatus, kbnEmployeeStatus);
            kbnEmployeeStatus.LastModifiedOn = DateTime.Now;
            context.KbnEmployeeStatus.Update(kbnEmployeeStatus);
            await context.SaveChangesAsync(cancellationToken);

            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
        }
    }
}

