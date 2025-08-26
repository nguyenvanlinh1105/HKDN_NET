using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.Employees;
using NineERP.Application.Dtos.User;
using NineERP.Application.Features.AuthFeature.Queries;
using NineERP.Application.Features.EmployeesFeature.Commands;
using NineERP.Application.Wrapper;

namespace NineERP.Web.Controllers.Api
{
    [ApiController]
    [Route("api/user")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserController(IMediator mediator, IValidator<EmployeeGeneralDto> validator) : ControllerBase
    {
        [HttpGet("me")]
        public async Task<IActionResult> GetCurrentUserInfo()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return new ObjectResult(GenericResponse<UserInfoDto>.ErrorResponse(401, ErrorMessages.GetMessage("AUTH005"), "AUTH005", ErrorMessages.GetMessage("AUTH005"))) { StatusCode = 401 };
            var response = await mediator.Send(new GetUserProfileQuery { Username = username });

            return new ObjectResult(response) { StatusCode = response.Status };
        }

        [HttpPost("me")]
        public async Task<IActionResult> PutCurrentUserInfo([FromBody] EmployeeGeneralDto request) 
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new ErrorDetail
                    {
                        Code = e.ErrorCode,
                        Details = e.ErrorMessage
                    })
                    .ToList();

                return new ObjectResult(GenericResponse<object>.MultipleErrorsResponse(400, "", errors)) { StatusCode = 400 };
            }

            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return new ObjectResult(GenericResponse<object>.ErrorResponse(401, ErrorMessages.GetMessage("AUTH005"), "AUTH005", ErrorMessages.GetMessage("AUTH005"))) { StatusCode = 401 };

            var response = await mediator.Send(new UpdateEmployeeGeneralCommand(request));

            return new ObjectResult(response) { StatusCode = response.Status };
        }
    }
}
