using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Constants.Messages;
using NineERP.Application.Dtos.PositionGeneral;
using NineERP.Application.Features.PositionFeature.Queries;
using NineERP.Application.Wrapper;

namespace NineERP.Web.Controllers.Api
{
    [ApiController]
    [Route("api/position")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PositionController(IMediator mediator) : ControllerBase
    {
        [HttpGet("user")]
        public async Task<IActionResult> GetPositionGeneralInfo()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
                return new ObjectResult(GenericResponse<PositionGeneralDto>.ErrorResponse(401, ErrorMessages.GetMessage("AUTH005"), "AUTH005", ErrorMessages.GetMessage("AUTH005"))) { StatusCode = 401 };
            var response = await mediator.Send(new GetPositionGeneralInfoQuery { EmployeeNo = username });

            return new ObjectResult(response) { StatusCode = response.Status };
        }
    }
}
