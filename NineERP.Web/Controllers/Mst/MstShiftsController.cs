using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Dtos.MstShift;
using NineERP.Application.Features.MstShiftFeature.Commands;
using NineERP.Application.Features.MstShiftFeature.Queries;
using NineERP.Domain.Enums;

namespace NineERP.Web.Controllers.Mst;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class MstShiftsController(IMediator mediator) : BaseController
{
    [Authorize(Policy = $"Permission:{PermissionValue.MstShifts.View}")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.MstShifts.View}")]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllMstShiftQuery());
        return Json(result);
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.MstShifts.View}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await mediator.Send(new GetMstShiftByIdQuery(id));
        return Json(result);
    }

    [HttpPost]
    [Authorize(Policy = $"Permission:{PermissionValue.MstShifts.Add}")]
    public async Task<IActionResult> Create([FromBody] CreateMstShiftCommand command)
    {
        var result = await mediator.Send(command);
        return Json(result);
    }

    [HttpPut]
    [Authorize(Policy = $"Permission:{PermissionValue.MstShifts.Update}")]
    public async Task<IActionResult> Update([FromBody] UpdateMstShiftCommand command)
    {
        var result = await mediator.Send(command);
        return Json(result);
    }

    [HttpPost]
    [Authorize(Policy = $"Permission:{PermissionValue.MstShifts.Delete}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteMstShiftCommand(id));
        return Json(result);
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.MstShifts.View}")]
    public async Task<IActionResult> GetAllPaging([FromQuery] MstShiftRequest request)
    {
        var result = await mediator.Send(new GetMstShiftPaginationQuery(request));
        return Json(result);
    }

    [HttpPut]
    [Authorize(Policy = $"Permission:{PermissionValue.MstShifts.Update}")]
    public async Task<IActionResult> SetDefault([FromBody] SetDefaultShiftRequest request)
    {
        var result = await mediator.Send(new SetDefaultMstShiftCommand(request.Id));
        return Json(result);
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.MstShifts.View}")]
    public async Task<IActionResult> GetEmployeesByShift(int shiftId)
    {
        var result = await mediator.Send(new GetEmployeesByShiftQuery(shiftId));
        return Ok(result);
    }

    [HttpPut]
    [Authorize(Policy = $"Permission:{PermissionValue.MstShifts.Update}")]
    public async Task<IActionResult> MoveEmployees([FromBody] MoveEmployeesRequest command)
    {
        var result = await mediator.Send(command);
        return Json(result);
    }
}
