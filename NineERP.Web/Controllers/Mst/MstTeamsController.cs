using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Dtos.MstTeam;
using NineERP.Application.Features.MstTeamFeature.Commands;
using NineERP.Application.Features.MstTeamFeature.Queries;
using NineERP.Domain.Enums;

namespace NineERP.Web.Controllers.Mst;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class MstTeamsController(IMediator mediator) : BaseController
{
    [Authorize(Policy = $"Permission:{PermissionValue.MstTeams.View}")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.MstTeams.View}")]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllMstTeamQuery());
        return Json(result);
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.MstTeams.View}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await mediator.Send(new GetMstTeamByIdQuery(id));
        return Json(result);
    }

    [HttpPost]
    [Authorize(Policy = $"Permission:{PermissionValue.MstTeams.Add}")]
    public async Task<IActionResult> Create([FromBody] CreateMstTeamCommand command)
    {
        var result = await mediator.Send(command);
        return Json(result);
    }

    [HttpPut]
    [Authorize(Policy = $"Permission:{PermissionValue.MstTeams.Update}")]
    public async Task<IActionResult> Update([FromBody] UpdateMstTeamCommand command)
    {
        var result = await mediator.Send(command);
        return Json(result);
    }

    [HttpPost]
    [Authorize(Policy = $"Permission:{PermissionValue.MstTeams.Delete}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteMstTeamCommand(id));
        return Json(result);
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.MstTeams.View}")]
    public async Task<IActionResult> GetAllPaging([FromQuery] MstTeamRequest request)
    {
        var result = await mediator.Send(new GetMstTeamPaginationQuery(request));
        return Json(result);
    }
}
