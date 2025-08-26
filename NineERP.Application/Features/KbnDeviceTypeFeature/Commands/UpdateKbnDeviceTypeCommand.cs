using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.KbnDeviceType;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.KbnDeviceTypeFeature.Commands;
public record UpdateKbnDeviceTypeCommand(KbnDeviceTypeDto kbnDeviceType) : IRequest<IResult>
{
    public class Handler(IApplicationDbContext context, IMapper mapper)
        : IRequestHandler<UpdateKbnDeviceTypeCommand, IResult>
    {
        public async Task<IResult> Handle(UpdateKbnDeviceTypeCommand request, CancellationToken cancellationToken)
        {
            var deviceType = await context.KbnDeviceTypes
                .FirstOrDefaultAsync(x => x.Id == request.kbnDeviceType.Id && !x.IsDeleted, cancellationToken);

            if (deviceType == null)
                return await Result.FailAsync(MessageConstants.NotFound);

            deviceType = mapper.Map(request.kbnDeviceType, deviceType);
            deviceType.LastModifiedOn = DateTime.Now;
            context.KbnDeviceTypes.Update(deviceType);
            await context.SaveChangesAsync(cancellationToken);

            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
        }
    }
}

