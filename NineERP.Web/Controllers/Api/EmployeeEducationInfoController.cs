using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.EducationInfo;
using NineERP.Application.Features.EmployeeEducationInfoFeature.Commands;
using NineERP.Application.Features.EmployeeEducationInfoFeature.Queries;
using NineERP.Application.Wrapper;

namespace NineERP.Web.Controllers.Api
{
    [ApiController]
    [Route("api/education-info")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class EmployeeEducationInfoController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetEducationInfo()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return new ObjectResult(GenericResponse<EducationInfoDto>.ErrorResponse(401, ErrorMessages.GetMessage("AUTH005"), "AUTH005", ErrorMessages.GetMessage("AUTH005"))) { StatusCode = 401 };
            var response = await mediator.Send(new GetEducationInfoQuery { EmployeeNo = username });

            return new ObjectResult(response) { StatusCode = response.Status };
        }

        [HttpPost]
        public async Task<IActionResult> PutEducationInfo([FromBody] EducationInfoDto request)
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return new ObjectResult(GenericResponse<object>.ErrorResponse(401, ErrorMessages.GetMessage("AUTH005"), "AUTH005", ErrorMessages.GetMessage("AUTH005"))) { StatusCode = 401 };
            
            var response = await mediator.Send(new UpdateEducationInfoCommand(request));

            return new ObjectResult(response) { StatusCode = response.Status };
        }
    }
}
