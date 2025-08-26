using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Dtos.DatCustomer;
using NineERP.Application.Features.DatCustomerFeature.Commands;
using NineERP.Application.Features.DatCustomerFeature.Queries;
using NineERP.Domain.Enums;

namespace NineERP.Web.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class CustomersController(IMediator mediator) : BaseController
{
    [Authorize(Policy = $"Permission:{PermissionValue.Customers.View}")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.Customers.View}")]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllDatCustomerQuery());
        return Json(result);
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.Customers.View}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await mediator.Send(new GetDatCustomerByIdQuery(id));
        return Json(result);
    }

    [HttpPost]
    [Authorize(Policy = $"Permission:{PermissionValue.Customers.Add}")]
    public async Task<IActionResult> Create([FromBody] CreateDatCustomerCommand command)
    {
        var result = await mediator.Send(command);
        return Json(result);
    }

    [HttpPut]
    [Authorize(Policy = $"Permission:{PermissionValue.Customers.Update}")]
    public async Task<IActionResult> Update([FromBody] UpdateDatCustomerCommand command)
    {
        var result = await mediator.Send(command);
        return Json(result);
    }

    [HttpPost]
    [Authorize(Policy = $"Permission:{PermissionValue.Customers.Delete}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteDatCustomerCommand(id));
        return Json(result);
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.Customers.View}")]
    public async Task<IActionResult> GetAllPaging([FromQuery] DatCustomerRequest request)
    {
        var result = await mediator.Send(new GetDatCustomerPaginationQuery(request));
        return Json(result);
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnEmployeeStatus.View}")]
    public async Task<IActionResult> GetStatus()
    {
        var result = await mediator.Send(new GetStatusQuery());
        return Json(result);
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnEmployeeStatus.View}")]
    public async Task<IActionResult> GetType()
    {
        var result = await mediator.Send(new GetTypeQuery());
        return Json(result);
    }
}
