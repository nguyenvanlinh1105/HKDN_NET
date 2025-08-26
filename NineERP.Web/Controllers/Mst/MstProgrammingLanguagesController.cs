using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Dtos.MstProgrammingLanguage;
using NineERP.Application.Features.MstProgrammingLanguageFeature.Commands;
using NineERP.Application.Features.MstProgrammingLanguageFeature.Queries;
using NineERP.Domain.Enums;

namespace NineERP.Web.Controllers.Mst;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class MstProgrammingLanguagesController(IMediator mediator) : BaseController
{
    [Authorize(Policy = $"Permission:{PermissionValue.MstProgrammingLanguages.View}")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.MstProgrammingLanguages.View}")]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllMstProgrammingLanguageQuery());
        return Json(result);
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.MstProgrammingLanguages.View}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await mediator.Send(new GetMstProgrammingLanguageByIdQuery(id));
        return Json(result);
    }

    [HttpPost]
    [Authorize(Policy = $"Permission:{PermissionValue.MstProgrammingLanguages.Add}")]
    public async Task<IActionResult> Create([FromBody] CreateMstProgrammingLanguageCommand command)
    {
        var result = await mediator.Send(command);
        return Json(result);
    }

    [HttpPut]
    [Authorize(Policy = $"Permission:{PermissionValue.MstProgrammingLanguages.Update}")]
    public async Task<IActionResult> Update([FromBody] UpdateMstProgrammingLanguageCommand command)
    {
        var result = await mediator.Send(command);
        return Json(result);
    }

    [HttpPost]
    [Authorize(Policy = $"Permission:{PermissionValue.MstProgrammingLanguages.Delete}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteMstProgrammingLanguageCommand(id));
        return Json(result);
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.MstProgrammingLanguages.View}")]
    public async Task<IActionResult> GetAllPaging([FromQuery] MstProgrammingLanguageRequest request)
    {
        var result = await mediator.Send(new GetMstProgrammingLanguagePaginationQuery(request));
        return Json(result);
    }
}
