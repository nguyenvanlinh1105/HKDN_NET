using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Features.ProjectFeature.Queries;

namespace NineERP.Web.Controllers.Api
{

    [ApiController]
    [Route("api/projects")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ProjectController(IMediator mediator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetProjects([FromQuery] GetProjectsQuery request)
        {
            var response = await mediator.Send(request);
            return new ObjectResult(response) { StatusCode = response.Status };
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProject(int id)
        {
            var response = await mediator.Send(new GetProjectQuery { Id = id });
            return new ObjectResult(response) { StatusCode = response.Status };
        }

        [HttpGet("participants")]
        public async Task<IActionResult> GetParticipants([FromQuery] GetParticipantsQuery request)
        {
            var response = await mediator.Send(request);
            return new ObjectResult(response) { StatusCode = response.Status };
        }
    }
}
