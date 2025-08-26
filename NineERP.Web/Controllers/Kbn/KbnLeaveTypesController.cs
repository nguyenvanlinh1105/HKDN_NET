using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Dtos.KbnLeaveType;
using NineERP.Application.Features.KbnLeaveTypeFeature.Commands;
using NineERP.Application.Features.KbnLeaveTypeFeature.Queries;
using NineERP.Domain.Enums;

namespace NineERP.Web.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class KbnLeaveTypesController(IMediator mediator) : BaseController
{
    [Authorize(Policy = $"Permission:{PermissionValue.KbnLeaveTypes.View}")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnLeaveTypes.View}")]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllKbnLeaveTypeQuery());
        return Json(result);
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnLeaveTypes.View}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await mediator.Send(new GetKbnLeaveTypeByIdQuery(id));
        return Json(result);
    }

    [HttpPost]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnLeaveTypes.Add}")]
    public async Task<IActionResult> Create([FromBody] CreateKbnLeaveTypeCommand command)
    {
        var result = await mediator.Send(command);
        return Json(result);
    }

    [HttpPut]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnLeaveTypes.Update}")]
    public async Task<IActionResult> Update([FromBody] UpdateKbnLeaveTypeCommand command)
    {
        var result = await mediator.Send(command);
        return Json(result);
    }

    [HttpPost]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnLeaveTypes.Delete}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteKbnLeaveTypeCommand(id));
        return Json(result);
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnLeaveTypes.View}")]
    public async Task<IActionResult> GetAllPaging([FromQuery] KbnLeaveTypeRequest request)
    {
        var result = await mediator.Send(new GetKbnLeaveTypePaginationQuery(request));
        return Json(result);
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnLeaveTypes.View}")]
    public async Task<IActionResult> GetLeaveTypeFlag()
    {
        var result = await mediator.Send(new GetLeaveTypeFlagQuery());
        return Json(result);
    }
}
