using MediatR;
using Microsoft.AspNetCore.Identity;
using NineERP.Application.Dtos.Identity.Requests;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Identity;

namespace NineERP.Application.Features.UsersFeature.Commands;

public class ChangePasswordCommand(ChangePasswordRequest model, string userId) : IRequest<IResult>
{
    public ChangePasswordRequest Model { get; set; } = model;
    public string UserId { get; set; } = userId;

    public class Handler(UserManager<AppUser> userManager) : IRequestHandler<ChangePasswordCommand, IResult>
    {
        public async Task<IResult> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            var user = await userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                return await Result.FailAsync("User Not Found.");
            }

            var identityResult = await userManager.ChangePasswordAsync(
                user,
                request.Model.Password,
                request.Model.NewPassword);

            var errors = identityResult.Errors.Select(e => e.Description).ToList();
            return identityResult.Succeeded ? await Result.SuccessAsync() : await Result.FailAsync(errors);
        }
    }
}