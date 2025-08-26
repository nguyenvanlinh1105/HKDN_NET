using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.MstTeam;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.MstTeamFeature.Commands;
public record UpdateMstTeamCommand(MstTeamDto MstTeam) : IRequest<IResult>
{
    public class Handler(IApplicationDbContext context, IMapper mapper)
        : IRequestHandler<UpdateMstTeamCommand, IResult>
    {
        public async Task<IResult> Handle(UpdateMstTeamCommand request, CancellationToken cancellationToken)
        {
            var mstTeam = await context.MstTeams
                .FirstOrDefaultAsync(x => x.Id == request.MstTeam.Id && !x.IsDeleted, cancellationToken);

            if (mstTeam == null)
                return await Result.FailAsync(MessageConstants.NotFound);

            mstTeam = mapper.Map(request.MstTeam, mstTeam);
            mstTeam.LastModifiedOn = DateTime.Now;
            context.MstTeams.Update(mstTeam);
            await context.SaveChangesAsync(cancellationToken);

            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
        }
    }
}

