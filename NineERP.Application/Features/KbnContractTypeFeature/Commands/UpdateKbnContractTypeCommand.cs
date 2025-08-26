using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.KbnContractType;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.KbnContractTypeFeature.Commands;
public record UpdateKbnContractTypeCommand(KbnContractTypeDto ContractType) : IRequest<IResult>
{
    public class Handler(IApplicationDbContext context, IMapper mapper)
        : IRequestHandler<UpdateKbnContractTypeCommand, IResult>
    {
        public async Task<IResult> Handle(UpdateKbnContractTypeCommand request, CancellationToken cancellationToken)
        {
            var contractType = await context.KbnContractTypes
                .FirstOrDefaultAsync(x => x.Id == request.ContractType.Id && !x.IsDeleted, cancellationToken);

            if (contractType == null)
                return await Result.FailAsync(MessageConstants.NotFound);

            contractType = mapper.Map(request.ContractType, contractType);
            contractType.LastModifiedOn = DateTime.Now;
            context.KbnContractTypes.Update(contractType);
            await context.SaveChangesAsync(cancellationToken);

            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
        }
    }
}

