using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Dtos.KbnDeviceType;
using NineERP.Application.Features.KbnDeviceTypeFeature.Commands;
using NineERP.Application.Features.KbnDeviceTypeFeature.Queries;
using NineERP.Domain.Enums;

namespace NineERP.Web.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class KbnDeviceTypesController(IMediator mediator) : BaseController
{
    [Authorize(Policy = $"Permission:{PermissionValue.KbnDeviceTypes.View}")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnDeviceTypes.View}")]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllKbnDeviceTypesQuery());
        return Json(result);
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnDeviceTypes.View}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await mediator.Send(new GetKbnDeviceTypeByIdQuery(id));
        return Json(result);
    }

    [HttpPost]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnDeviceTypes.Add}")]
    public async Task<IActionResult> Create([FromBody] CreateKbnDeviceTypeCommand command)
    {
        var result = await mediator.Send(command);
        return Json(result);
    }

    [HttpPut]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnDeviceTypes.Update}")]
    public async Task<IActionResult> Update([FromBody] UpdateKbnDeviceTypeCommand command)
    {
        var result = await mediator.Send(command);
        return Json(result);
    }

    [HttpPost]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnDeviceTypes.Delete}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteKbnDeviceTypeCommand(id));
        return Json(result);
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnDeviceTypes.View}")]
    public async Task<IActionResult> GetAllPaging([FromQuery] KbnDeviceTypeRequest request)
    {
        var result = await mediator.Send(new GetKbnDeviceTypePaginationQuery(request));
        return Json(result);
    }
}
