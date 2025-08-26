using NineERP.Application.Dtos.Identity.Requests;
using NineERP.Domain.Entities.Identity;
using NineERP.Application.Wrapper;
using Hangfire;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.Encodings.Web;
using NineERP.Application.Constants.Messages;
using NineERP.Infrastructure.Helpers;
using NineERP.Application.Interfaces.Identity;
using NineERP.Application.Constants.Role;
using NineERP.Application.Constants.User;
using NineERP.Application.Extensions;
using NineERP.Application.Interfaces.Common;

namespace NineERP.Infrastructure.Services.Identity
{
    public class IdentityService(
        UserManager<AppUser> userManager,
        IEmailService mailService)
        : ITokenService
    {
        public async Task<IResult> RegisterAsync(RegisterRequest request, string origin)
        {
            var user = new AppUser
            {
                Email = request.Email,
                FullName = request.FullName,
                UserName = request.Email,
                PhoneNumber = request.PhoneNumber,
                EmailConfirmed = false,
            };

            var userWithSameEmail = await userManager.FindByEmailAsync(request.Email);

            if (userWithSameEmail == null)
            {
                var result = await userManager.CreateAsync(user, UserConstants.DefaultPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, RoleConstants.User);
                    // Send email to user for confirmation
                    var verificationUri = await SendVerificationEmail(user, origin);
                    var body = @"
                        <!DOCTYPE html>
                        <html lang='en'>
                        <head>
                            <meta charset='UTF-8'>
                            <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                            <title>Document</title>
                            <style>
                                .tm-footer p {
                                    color: #646464;
                                }
                                .tm-footer span {
                                    color: #171717;
                                }
                                .tm-footer a {
                                    color: #0F723A;
                                }
                                .wrap-confirm {
                                    width: 575px;
                                    margin: 0 auto;
                                    background-color: #fff;
                                    padding: 20px;
                                }
                            </style>
                        </head>
                        <body>
                            <div class='wrap-confirm'>
                                <div class='logo' style='margin-bottom: 25px;'>
                                    <img src='https://minmaxgreen.vn/assets/landing-page/img/logo/logo-png.png' alt=''>
                                </div>
                                <div class='tm-body' style='margin-bottom: 100px;'>
                                    <p style='font-weight: bold;'>MinMax Green sincerely thanks for registering an account.</p>
                                    <p style='color: #646464;'>Please kindly confirm your account by <a style='color: #171717;' href='verificationUri'>click here.</a></p>
                                </div>
                                <div class='tm-footer' style='font-size: 14px;'>
                                    <p>Gmail:<span> team@minmaxgreen.vn </span></p>
                                    <p>Phone Number: <span> +84 931 861 313</span></p>
                                    <p>Address:<span>68 Nguyen Hue, Ward Ben Nghe, District 1, Ho Chi Minh City, Vietnam</span></p>
                                    <p>Website: <a href='http://minmaxgreen.vn/'>minmaxgreen.vn</a></p>
                                    <p style='font-size: 13px;'>Copyright © 2023 Co2 Project All rights reserved.</p>
                                </div>
                            </div>
                        </body>
                        </html>";
                    var replace = body.Replace("verificationUri", verificationUri);
                    BackgroundJob.Enqueue(() => mailService.SendAsync(
                        user.Email,
                        "Confirm Registration",
                        replace
                    ));

                    return await Result<string>.SuccessAsync(user.Id,
                        $"User with email {user.Email} Registered. Please check your Mailbox to verify!");
                }

                return await Result.FailAsync(MessageConstants.AddFail);
            }

            return await Result.FailAsync("EmailIsAlreadyRegistered");
        }

        public async Task<IResult<AppUser>> RegisterWithGoogleV2Async(RegisterRequest request)
        {
            var user = new AppUser
            {
                Email = request.Email,
                FullName = request.FullName,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                EmailConfirmed = true,
            };

            var userWithSameEmail = await userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail != null) return await Result<AppUser>.FailAsync("EmailIsAlreadyRegistered");

            var result = await userManager.CreateAsync(user, UserConstants.DefaultPassword);
            if (!result.Succeeded) return await Result<AppUser>.FailAsync(MessageConstants.AddFail);

            await userManager.AddToRoleAsync(user, RoleConstants.User);
            return await Result<AppUser>.SuccessAsync(MessageConstants.AddSuccess);
        }

        private async Task<string> SendVerificationEmail(AppUser user, string origin)
        {
            var code = await userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            const string route = "identity/confirm-email";
            var endpointUri = new Uri(string.Concat($"{origin}/", route));
            var verificationUri = QueryHelpers.AddQueryString(endpointUri.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
            return verificationUri;
        }

        public async Task<IResult<string>> ConfirmEmailAsync(string userId, string code)
        {
            var user = await userManager.FindByIdAsync(userId);
            if (user == null || user.IsDeleted)
            {
                return await Result<string>.FailAsync(MessageConstants.NotFound);
            }
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return await Result<string>.SuccessAsync(user.Id, $"Account Confirmed for {user.Email}.");
            }

            throw new ApiException($"An error occurred while confirming {user.Email}");
        }

        public async Task<IResult> ForgotPasswordAsync(ForgotPasswordRequest request, string origin)
        {
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null || !(await userManager.IsEmailConfirmedAsync(user)) || user.IsDeleted)
            {
                // Don't reveal that the user does not exist or is not confirmed
                return await Result.FailAsync("An Error has occurred!");
            }
            // For more information on how to enable account confirmation and password reset please
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            var code = await userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            const string route = "identity/reset-password";
            var endpointUri = new Uri(string.Concat($"{origin}/", route));
            var passwordResetUrl = QueryHelpers.AddQueryString(endpointUri.ToString(), "Token", code) + "&Email=" + request.Email;
            var body = @"
                    <!DOCTYPE html>
                    <html lang='en'>
                    <head>
                        <meta charset='UTF-8'>
                        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
                        <title>Document</title>
                        <style>
                            .tm-footer p {
                                color: #646464;
                            }
                            .tm-footer span {
                                color: #171717;
                            }
                            .tm-footer a {
                                color: #0F723A;
                            }
                            .wrap-confirm {
                                width: 575px;
                                margin: 0 auto;
                                background-color: #fff;
                                padding: 20px;
                            }
                        </style>
                    </head>
                    <body>
                        <div class='wrap-confirm'>
                            <div class='logo' style='margin-bottom: 25px;'>
                                <img src='https://minmaxgreen.vn/assets/landing-page/img/logo/logo-png.png' alt=''>
                            </div>
                            <div class='tm-body' style='margin-bottom: 50px;'>
                                <p style='font-weight: bold;'>We received a request to restore login information for your MinMax Green account..</p>
                                <p style='color: #646464;'>Please reset your password by <a style='color: #171717;' href='verificationUri'>click here.</a></p>
                            </div>
                            <div class='tm-footer' style='font-size: 14px;'>
                                <p>Gmail:<span> team@minmaxgreen.vn </span></p>
                                <p>Phone Number: <span> +84 931 861 313</span></p>
                                <p>Address:<span>68 Nguyen Hue, Ward Ben Nghe, District 1, Ho Chi Minh City, Vietnam</span></p>
                                <p>Website: <a href='http://minmaxgreen.vn/'>minmaxgreen.vn</a></p>
                                <p style='font-size: 13px;'>Copyright © 2023 Co2 Project All rights reserved.</p>
                            </div>
                        </div>
                    </body>
                    </html>";

            var replace = body.Replace("verificationUri", HtmlEncoder.Default.Encode(passwordResetUrl));

            BackgroundJob.Enqueue(() => mailService.SendAsync(
                request.Email,
                "Reset Password",
                replace
            ));

            return await Result.SuccessAsync("Password Reset Mail has been sent to your authorized Email.");
        }

        public async Task<IResult> ResetPasswordAsync(ResetPasswordRequest request)
        {
            //find user
            var user = await userManager.FindByEmailAsync(request.Email);
            if (user == null || user.IsDeleted)
            {
                return await Result.FailAsync("An Error has occured!");
            }
            //check password match
            if (!request.Password.Equals(request.ConfirmPassword))
            {
                return await Result.FailAsync("Confirm Password not match!");
            }
            //check validate
            if (!FileHelper.ValidatePassword(request.Password))
            {
                return await Result<string>.FailAsync(MessageConstants.ValidationPassword);
            }
            //reset
            var result = await userManager.ResetPasswordAsync(user, request.Token, request.Password);
            if (result.Succeeded)
            {
                return await Result.SuccessAsync("Password Reset Successful!");
            }

            return await Result.FailAsync("An Error has occured!");
        }

        public async Task<IResult<AppUser?>> RegisterWithGoogleAsync(RegisterRequest request)
        {
            var user = new AppUser
            {
                Email = request.Email,
                FullName = request.FullName,
                UserName = request.UserName,
                PhoneNumber = request.PhoneNumber,
                EmailConfirmed = true,
            };

            var userWithSameEmail = await userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail == null)
            {
                var result = await userManager.CreateAsync(user, UserConstants.DefaultPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, RoleConstants.User);
                    return await Result<AppUser>.SuccessAsync(user);
                }

                return await Result<AppUser>.FailAsync(result.Errors.Select(a => a.Description.ToString()).ToList());
            }

            return await Result<AppUser>.SuccessAsync(userWithSameEmail);
        }
    }
}