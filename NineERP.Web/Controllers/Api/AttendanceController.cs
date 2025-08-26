using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Features.EmployeeLogTimeFeature.Commands;
using NineERP.Application.Features.EmployeeLogTimeFeature.Queries.GetHistoryAttendanceCurrentUserQuery;
using NineERP.Application.Features.EmployeeLogTimeFeature.Queries.GetStatusQuery;
using static NineERP.Application.Constants.Global.GlobalConstants;

namespace NineERP.Web.Controllers.Api
{
    [ApiController]
    [Route("api/attendance")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AttendanceController(IMediator mediator) : ControllerBase
    {
        [HttpPost("check-in")]
        public async Task<IActionResult> CheckIn()
        {
            var response = await mediator.Send(new AddEmployeeLogTimeCommand { Type = TimeLogType.CheckIn.ToString() });
            return new ObjectResult(response) { StatusCode = response.Status };
        }

        [HttpPost("check-out")]
        public async Task<IActionResult> CheckOut()
        {
            var response = await mediator.Send(new AddEmployeeLogTimeCommand { Type = TimeLogType.CheckOut.ToString() });
            return new ObjectResult(response) { StatusCode = response.Status };
        }

        [HttpGet("status")]
        public async Task<IActionResult> GetCheckInStatus()
        {
            var response = await mediator.Send(new GetStatusQuery());
            return new ObjectResult(response) { StatusCode = response.Status };
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetHistoryLogTimeCurrentUser([FromQuery] GetHistoryLogTimeCurrentUserQuery request)
        {
            var response = await mediator.Send(request);
            return new ObjectResult(response) { StatusCode = response.Status };
        }

        [HttpPost]
        public async Task<IActionResult> PutAttendance([FromBody] EditEmployeeLogTimeCommand request)
        {
            var response = await mediator.Send(request);
            return new ObjectResult(response) { StatusCode = response.Status };
        }
    }

}
