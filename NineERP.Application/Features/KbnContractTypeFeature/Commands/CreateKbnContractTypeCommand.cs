using AutoMapper;
using MediatR;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.KbnContractType;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Kbn;

namespace NineERP.Application.Features.KbnContractTypeFeature.Commands;
public record CreateKbnContractTypeCommand(KbnContractTypeDto ContractType) : IRequest<IResult>
{
    public class CreateContractTypeCommandHandler(
        IApplicationDbContext context,
        IMapper mapper) : IRequestHandler<CreateKbnContractTypeCommand, IResult>
    {
        public async Task<IResult> Handle(CreateKbnContractTypeCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await context.BeginTransactionAsync(cancellationToken);
            try
            {
                // 1. Tạo KbnContractType
                var contractType = mapper.Map<KbnContractType>(request.ContractType);
                contractType.CreatedOn = DateTime.Now;
                context.KbnContractTypes.Add(contractType);

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
