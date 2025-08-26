using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.KbnHolidayType;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.KbnHolidayTypeFeature.Commands;
public record UpdateKbnHolidayTypeCommand(KbnHolidayTypeDto KbnHolidayType) : IRequest<IResult>
{
    public class Handler(IApplicationDbContext context, IMapper mapper)
        : IRequestHandler<UpdateKbnHolidayTypeCommand, IResult>
    {
        public async Task<IResult> Handle(UpdateKbnHolidayTypeCommand request, CancellationToken cancellationToken)
        {
            var kbnHolidayType = await context.KbnHolidayTypes
                .FirstOrDefaultAsync(x => x.Id == request.KbnHolidayType.Id && !x.IsDeleted, cancellationToken);

            if (kbnHolidayType == null)
                return await Result.FailAsync(MessageConstants.NotFound);

            kbnHolidayType = mapper.Map(request.KbnHolidayType, kbnHolidayType);
            kbnHolidayType.LastModifiedOn = DateTime.Now;
            context.KbnHolidayTypes.Update(kbnHolidayType);
            await context.SaveChangesAsync(cancellationToken);

            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
        }
    }
}

