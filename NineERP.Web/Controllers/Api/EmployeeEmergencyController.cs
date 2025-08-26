using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.EmergencyContact;
using NineERP.Application.Dtos.User;
using NineERP.Application.Features.EmployeeEmergencyFeature.Commands;
using NineERP.Application.Features.EmployeeEmergencyFeature.Queries;
using NineERP.Application.Wrapper;

namespace NineERP.Web.Controllers.Api
{

    [ApiController]
    [Route("api/emergency-contact")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EmployeeEmergencyController(IMediator mediator, IValidator<EmergencyContactDto> validator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetEmergencyContact()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return new ObjectResult(GenericResponse<UserInfoDto>.ErrorResponse(401, ErrorMessages.GetMessage("AUTH005"), "AUTH005", ErrorMessages.GetMessage("AUTH005"))) { StatusCode = 401 };
            var response = await mediator.Send(new GetEmergencyContactQuery { EmployeeNo = username });

            return new ObjectResult(response) { StatusCode = response.Status };
        }

        [HttpPost]
        public async Task<IActionResult> PutEmergencyContact([FromBody] EmergencyContactDto request)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return new ObjectResult(GenericResponse<object>.ErrorResponse(401, ErrorMessages.GetMessage("AUTH005"), "AUTH005", ErrorMessages.GetMessage("AUTH005"))) { StatusCode = 401 };
            
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
            var response = await mediator.Send(new UpdateEmergencyContactCommand(request));

            return new ObjectResult(response) { StatusCode = response.Status };
        }
    }
}
