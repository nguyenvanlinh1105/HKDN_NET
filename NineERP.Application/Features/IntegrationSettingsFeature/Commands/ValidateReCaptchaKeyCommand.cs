using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json.Serialization;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using NineERP.Domain.Enums;

namespace NineERP.Application.Features.IntegrationSettingsFeature.Commands
{
    public class ValidateReCaptchaKeyCommand : IRequest<IResult>
    {
    }

    public class Handler(IApplicationDbContext context, IHttpClientFactory httpClientFactory)
        : IRequestHandler<ValidateReCaptchaKeyCommand, IResult>
    {
        public async Task<IResult> Handle(ValidateReCaptchaKeyCommand request, CancellationToken cancellationToken)
        {
            var setting = await context.IntegrationSettings
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Type == IntegrationServiceType.ReCaptcha && x.IsActive, cancellationToken);

            if (setting == null || string.IsNullOrWhiteSpace(setting.PublicKey) || string.IsNullOrWhiteSpace(setting.PrivateKey))
            {
                return await Result.FailAsync("Missing or invalid ReCaptcha configuration.");
            }

            var httpClient = httpClientFactory.CreateClient();

            var response = await httpClient.PostAsync(
                "https://www.google.com/recaptcha/api/siteverify",
                new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    { "secret", setting.PrivateKey },
                    { "response", "dummy" } // test with invalid token
                }), cancellationToken
            );

            if (!response.IsSuccessStatusCode)
            {
                return await Result.FailAsync("reCAPTCHA API call failed.");
            }

            var apiResult = await response.Content.ReadFromJsonAsync<ReCaptchaApiResponse>(cancellationToken: cancellationToken);

            // If we receive a valid response, even with 'success = false', we know the keys are valid
            if (apiResult != null && apiResult.ErrorCodes is not null && apiResult.ErrorCodes.Any())
            {
                return await Result.SuccessAsync("ReCaptcha keys are valid but test token was invalid (expected).");
            }

            return await Result.FailAsync("Unexpected error. Could not validate ReCaptcha keys.");
        }

        private class ReCaptchaApiResponse
        {
            [JsonPropertyName("success")]
            public bool Success { get; set; }

            [JsonPropertyName("challenge_ts")]
            public string? ChallengeTs { get; set; }

            [JsonPropertyName("hostname")]
            public string? Hostname { get; set; }

            [JsonPropertyName("error-codes")]
            public List<string>? ErrorCodes { get; set; }
        }
    }
}
