using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Dtos.MstDepartment;
using NineERP.Application.Features.MstDepartmentsFeature.Commands;
using NineERP.Application.Features.MstDepartmentsFeature.Queries;
using NineERP.Domain.Enums;

namespace NineERP.Web.Controllers;

[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
public class DepartmentsController(IMediator mediator) : BaseController
{
    [Authorize(Policy = $"Permission:{PermissionValue.Departments.View}")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await mediator.Send(new GetAllDepartmentsQuery());
        return Json(result);
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.Departments.View}")]
    public async Task<IActionResult> GetTree()
    {
        var result = await mediator.Send(new GetDepartmentTreeQuery());

        if (!result.Succeeded || result.Data == null || result.Data.Count == 0)
            return Json(new { succeeded = false });

        // Lấy ngôn ngữ từ cookie
        var lang = HttpContext.Request.Cookies["lang"]?.ToLower() ?? "vi";
        if (lang != "vi" && lang != "en" && lang != "ja")
            lang = "vi";

        var treantFormat = new
        {
            chart = new { container = "#org-chart-container" },
            nodeStructure = ConvertToTreant(result.Data.First(), lang)
        };

        return Json(new { succeeded = true, data = treantFormat });
    }

    private object ConvertToTreant(DepartmentTreeDto node, string lang)
    {
        var name = lang switch
        {
            "en" => node.NameEn,
            "ja" => node.NameJa,
            _ => node.NameVi
        };

        var obj = new Dictionary<string, object>
        {
            ["text"] = new { name },
            ["HTMLclass"] = "nodeExample1"
        };

        if (node.Children?.Count > 0)
        {
            obj["children"] = node.Children.Select(child => ConvertToTreant(child, lang)).ToList();
        }

        return obj;
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.Departments.View}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await mediator.Send(new GetDepartmentByIdQuery(id));
        return Json(result);
    }

    [HttpPost]
    [Authorize(Policy = $"Permission:{PermissionValue.Departments.Add}")]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentCommand command)
    {
        var result = await mediator.Send(command);
        return Json(result);
    }

    [HttpPut]
    [Authorize(Policy = $"Permission:{PermissionValue.Departments.Update}")]
    public async Task<IActionResult> Update([FromBody] UpdateDepartmentCommand command)
    {
        var result = await mediator.Send(command);
        return Json(result);
    }

    [HttpPost]
    [Authorize(Policy = $"Permission:{PermissionValue.Departments.Delete}")]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await mediator.Send(new DeleteDepartmentCommand(id));
        return Json(result);
    }

    [HttpPost]
    [Authorize(Policy = $"Permission:{PermissionValue.Departments.Delete}")]
    public async Task<IActionResult> DeleteMultiple([FromBody] List<int> ids)
    {
        var result = await mediator.Send(new DeleteMultipleDepartmentsCommand(ids));
        return Json(result);
    }

    [HttpGet]
    [Authorize(Policy = $"Permission:{PermissionValue.Departments.View}")]
    public async Task<IActionResult> GetAllPaging([FromQuery] DepartmentRequest request)
    {
        var result = await mediator.Send(new GetDepartmentsPaginationQuery(request));
        return Json(result);
    }
}
