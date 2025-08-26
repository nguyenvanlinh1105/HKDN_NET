using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.User;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.UsersFeature.Queries;

public record GetUserByIdQuery(string Id) : IRequest<Result<UserDetailDto>>
{
    public class Handler(IApplicationDbContext context, IMapper mapper)
        : IRequestHandler<GetUserByIdQuery, Result<UserDetailDto>>
    {
        public async Task<Result<UserDetailDto>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
        {
            var user = await context.Users.FirstOrDefaultAsync(x => !x.IsDeleted && x.Id == request.Id, cancellationToken);
            var result = mapper.Map<UserDetailDto>(user);
            return await Result<UserDetailDto>.SuccessAsync(result ?? new UserDetailDto());
        }
    }
}
