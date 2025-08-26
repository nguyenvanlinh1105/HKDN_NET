using NineERP.Application.Dtos.Identity.Requests;
using NineERP.Domain.Entities.Identity;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Interfaces.Identity
{
    public interface ITokenService
    {
        Task<IResult> RegisterAsync(RegisterRequest request, string origin);

        Task<IResult<AppUser>> RegisterWithGoogleV2Async(RegisterRequest request);

        Task<IResult<string>> ConfirmEmailAsync(string userId, string code);

        Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin);

        Task<IResult> ResetPasswordAsync(ResetPasswordRequest request);

        Task<IResult<AppUser?>> RegisterWithGoogleAsync(RegisterRequest request);
    }
}