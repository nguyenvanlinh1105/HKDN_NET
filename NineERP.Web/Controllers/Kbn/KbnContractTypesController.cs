using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Dtos.KbnContractType;
using NineERP.Application.Features.KbnContractTypeFeature.Commands;
using NineERP.Application.Features.KbnContractTypeFeature.Queries;
using NineERP.Domain.Enums;

namespace NineERP.Web.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class KbnContractTypesController(IMediator mediator) : BaseController
{
    [Authorize(Policy = $"Permission:{PermissionValue.KbnContractTypes.View}")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllKbnContractTypesQuery());
        return Json(result);
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnContractTypes.View}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await mediator.Send(new GetKbnContractTypeByIdQuery(id));
        return Json(result);
    }

    [HttpPost]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnContractTypes.Add}")]
    public async Task<IActionResult> Create([FromBody] CreateKbnContractTypeCommand command)
    {
        var result = await mediator.Send(command);
        return Json(result);
    }

    [HttpPut]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnContractTypes.Update}")]
    public async Task<IActionResult> Update([FromBody] UpdateKbnContractTypeCommand command)
    {
        var result = await mediator.Send(command);
        return Json(result);
    }

    [HttpPost]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnContractTypes.Delete}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteKbnContractTypeCommand(id));
        return Json(result);
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.KbnContractTypes.View}")]
    public async Task<IActionResult> GetAllPaging([FromQuery] KbnContractTypeRequest request)
    {
        var result = await mediator.Send(new GetKbnContractTypePaginationQuery(request));
        return Json(result);
    }
}
