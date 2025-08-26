using AutoMapper;
using MediatR;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.DatCustomer;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Dat;

namespace NineERP.Application.Features.DatCustomerFeature.Commands;
public record CreateDatCustomerCommand(DatCustomerDto DatCustomer) : IRequest<IResult>
{
    public class CreateDeviceTypeCommandHandler(
        IApplicationDbContext context,
        IMapper mapper) : IRequestHandler<CreateDatCustomerCommand, IResult>
    {
        public async Task<IResult> Handle(CreateDatCustomerCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await context.BeginTransactionAsync(cancellationToken);
            try
            {
                // 1. Tạo DatCustomer
                var datCustomer = mapper.Map<DatCustomer>(request.DatCustomer);
                datCustomer.CreatedOn = DateTime.Now;
                context.DatCustomers.Add(datCustomer);

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
