using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.User;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.UsersFeature.Commands;

public record UpdateUserCommand(UserDetailDto UserDto) : IRequest<IResult>
{
    public class Handler(IApplicationDbContext context, IMapper mapper)
        : IRequestHandler<UpdateUserCommand, IResult>
    {
        public async Task<IResult> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == request.UserDto.Id, cancellationToken);
            if (user == null) return await Result.FailAsync(MessageConstants.NotFound);

            user = mapper.Map(request.UserDto, user);
            context.Users.Update(user);
            await context.SaveChangesAsync(cancellationToken);

            return await Result.SuccessAsync(MessageConstants.UpdateSuccess);
        }
    }
}