using AutoMapper;
using MediatR;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.MstTeam;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Mst;

namespace NineERP.Application.Features.MstTeamFeature.Commands;
public record CreateMstTeamCommand(MstTeamDto MstTeam) : IRequest<IResult>
{
    public class CreateDeviceTypeCommandHandler(
        IApplicationDbContext context,
        IMapper mapper) : IRequestHandler<CreateMstTeamCommand, IResult>
    {
        public async Task<IResult> Handle(CreateMstTeamCommand request, CancellationToken cancellationToken)
        {
            await using var transaction = await context.BeginTransactionAsync(cancellationToken);
            try
            {
                // 1. Tạo MstTeam
                var mstTeam = mapper.Map<MstTeam>(request.MstTeam);
                mstTeam.CreatedOn = DateTime.Now;
                context.MstTeams.Add(mstTeam);

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
