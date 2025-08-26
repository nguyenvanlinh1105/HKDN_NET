using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Dtos.KbnEmployeeStatus;
using NineERP.Application.Features.KbnEmployeeStatusFeature.Commands;
using NineERP.Application.Features.KbnEmployeeStatusFeature.Queries;
using NineERP.Domain.Enums;

namespace NineERP.Web.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class KbnEmployeeStatusController(IMediator mediator) : BaseController
{
    [Authorize(Policy = $"Permission:{PermissionValue.KbnEmployeeStatus.View}")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllKbnEmployeeStatusQuery());
        return Json(result);
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnEmployeeStatus.View}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await mediator.Send(new GetKbnEmployeeStatusByIdQuery(id));
        return Json(result);
    }

    [HttpPost]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnEmployeeStatus.Add}")]
    public async Task<IActionResult> Create([FromBody] CreateKbnEmployeeStatusCommand command)
    {
        var result = await mediator.Send(command);
        return Json(result);
    }

    [HttpPut]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnEmployeeStatus.Update}")]
    public async Task<IActionResult> Update([FromBody] UpdateKbnEmployeeStatusCommand command)
    {
        var result = await mediator.Send(command);
        return Json(result);
    }

    [HttpPost]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnEmployeeStatus.Delete}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteKbnEmployeeStatusCommand(id));
        return Json(result);
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnEmployeeStatus.View}")]
    public async Task<IActionResult> GetAllPaging([FromQuery] KbnEmployeeStatusRequest request)
    {
        var result = await mediator.Send(new GetKbnEmployeeStatusPaginationQuery(request));
        return Json(result);
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnEmployeeStatus.View}")]
    public async Task<IActionResult> GetStatus()
    {
        var result = await mediator.Send(new GetStatusQuery());
        return Json(result);
    }
}
