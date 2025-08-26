using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.DatCustomer;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.DatCustomerFeature.Commands;
public record UpdateDatCustomerCommand(DatCustomerDto DatCustomer) : IRequest<IResult>
{
    public class Handler(IApplicationDbContext context, IMapper mapper)
        : IRequestHandler<UpdateDatCustomerCommand, IResult>
    {
        public async Task<IResult> Handle(UpdateDatCustomerCommand request, CancellationToken cancellationToken)
        {
            var datCustomer = await context.DatCustomers
                .FirstOrDefaultAsync(x => x.Id == request.DatCustomer.Id && !x.IsDeleted, cancellationToken);

            if (datCustomer == null)
                return await Result.FailAsync(MessageConstants.NotFound);

            datCustomer = mapper.Map(request.DatCustomer, datCustomer);
            datCustomer.LastModifiedOn = DateTime.Now;
            context.DatCustomers.Update(datCustomer);
            await context.SaveChangesAsync(cancellationToken);

            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
        }
    }
}

