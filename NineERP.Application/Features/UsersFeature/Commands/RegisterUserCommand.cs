using Hangfire;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Constants.User;
using NineERP.Application.Dtos.Identity.Requests;
using NineERP.Application.Interfaces.Common;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Entities.Identity;
namespace NineERP.Application.Features.UsersFeature.Commands;

public class RegisterUserCommand(RegisterRequest request, string origin) : IRequest<IResult>
{
    public RegisterRequest Request { get; set; } = request;
    public string Origin { get; set; } = origin;

    public class Handler(
        UserManager<AppUser> userManager,
        IEmailService mailService,
        IApplicationDbContext context
    ) : IRequestHandler<RegisterUserCommand, IResult>
    {
        public async Task<IResult> Handle(RegisterUserCommand command, CancellationToken cancellationToken)
        {
            var request = command.Request;
            var origin = command.Origin;

            await using var transaction = await context.BeginTransactionAsync(cancellationToken);

            try
            {
                var user = new AppUser
                {
                    Email = request.Email,
                    FullName = request.FullName,
                    UserName = request.UserName,
                    PhoneNumber = request.PhoneNumber,
                    EmailConfirmed = true,
                    LockoutEnabled = false,
                    CreatedBy = "SuperAdmin"
                };

                if (await userManager.FindByEmailAsync(user.Email) is not null)
                    return await Result.FailAsync("EmailIsAlreadyRegistered");

                var result = await userManager.CreateAsync(user, UserConstants.DefaultPassword);
                if (!result.Succeeded)
                    return await Result.FailAsync(MessageConstants.AddFail);

                if (!string.IsNullOrEmpty(request.Role))
                    await userManager.AddToRoleAsync(user, request.Role);

                var template = await context.EmailTemplates
                    .Where(x => x.Code == "CONFIRM_EMAIL" && x.Language == "en" && x.IsActive)
                    .FirstOrDefaultAsync(cancellationToken);

                if (template is null)
                    return await Result.FailAsync("Email template not found.");

                var subject = template.Subject;
                var body = template.Body
                    .Replace("{{FullName}}", user.FullName)
                    .Replace("{{Email}}", user.Email)
                    .Replace("{{Password}}", UserConstants.DefaultPassword)
                    .Replace("{{LoginLink}}", $"{origin}/login");

                await context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                // Gửi mail sau khi transaction commit
                BackgroundJob.Enqueue(() => mailService.SendAsync(user.Email, subject, body));

                return await Result<string>.SuccessAsync(user.Id, $"User {user.Email} registered and notified.");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync(cancellationToken);
                return await Result.FailAsync($"An error occurred while registering user: {ex.Message}");
            }
        }
    }
}