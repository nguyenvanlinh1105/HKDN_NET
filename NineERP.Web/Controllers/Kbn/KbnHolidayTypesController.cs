using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Dtos.KbnHolidayType;
using NineERP.Application.Features.KbnHolidayTypeFeature.Commands;
using NineERP.Application.Features.KbnHolidayTypeFeature.Queries;
using NineERP.Domain.Enums;

namespace NineERP.Web.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class KbnHolidayTypesController(IMediator mediator) : BaseController
{
    [Authorize(Policy = $"Permission:{PermissionValue.KbnHolidayTypes.View}")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnHolidayTypes.View}")]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllKbnHolidayTypesQuery());
        return Json(result);
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnHolidayTypes.View}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await mediator.Send(new GetKbnHolidayTypeByIdQuery(id));
        return Json(result);
    }

    [HttpPost]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnHolidayTypes.Add}")]
    public async Task<IActionResult> Create([FromBody] CreateKbnHolidayTypeCommand command)
    {
        var result = await mediator.Send(command);
        return Json(result);
    }

    [HttpPut]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnHolidayTypes.Update}")]
    public async Task<IActionResult> Update([FromBody] UpdateKbnHolidayTypeCommand command)
    {
        var result = await mediator.Send(command);
        return Json(result);
    }

    [HttpPost]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnHolidayTypes.Delete}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteKbnHolidayTypeCommand(id));
        return Json(result);
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnHolidayTypes.View}")]
    public async Task<IActionResult> GetAllPaging([FromQuery] KbnHolidayTypeRequest request)
    {
        var result = await mediator.Send(new GetKbnHolidayTypesPaginationQuery(request));
        return Json(result);
    }
}
