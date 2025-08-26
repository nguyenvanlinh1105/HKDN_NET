using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Features.PositionFeature.Queries;
using NineERP.Application.Features.TaskFeature.Queries;

namespace NineERP.Web.Controllers.Api
{
    [ApiController]
    [Route("api/common")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CommonController(IMediator mediator) : ControllerBase
    {
        [HttpGet("positions")]
        public async Task<IActionResult> GetPosition()
        {
            var result = await mediator.Send(new GetPositionsQuery());
            return new ObjectResult(result) { StatusCode = result.Status };
        }

        [HttpGet("data-master-filter-task/{projectId}")]
        public async Task<IActionResult> GetDataMasterFilterTask(int projectId)
        {
            var result = await mediator.Send(new GetDataMasterFilterQuery { ProjectId = projectId });
            return new ObjectResult(result) { StatusCode = result.Status };
        }
    }
}
