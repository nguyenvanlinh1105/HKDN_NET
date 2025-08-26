using MediatR;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using NineERP.Application.Dtos.Employees;
using NineERP.Application.Features.EmployeesFeature.Commands;
using NineERP.Application.Features.EmployeesFeature.Queries;
using NineERP.Domain.Enums;

namespace NineERP.Web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class EmployeesController(IMediator mediator) : BaseController
    {
        [Authorize(Policy = $"Permission:{PermissionValue.Employees.View}")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [Authorize(Policy = $"Permission:{PermissionValue.Employees.View}")]
        public async Task<IActionResult> GetAllPaging([FromQuery] EmployeeRequest request)
        {
            var result = await mediator.Send(new GetEmployeesPaginationQuery(request));
            return Json(result);
        }

        [HttpGet]
        [Authorize(Policy = $"Permission:{PermissionValue.Employees.View}")]
        public async Task<IActionResult> GetById(long id)
        {
            var result = await mediator.Send(new GetEmployeeByIdQuery(id));
            return Json(result);
        }

        [HttpGet("employees/detail/{id}")]
        [Authorize(Policy = $"Permission:{PermissionValue.Employees.View}")]
        public async Task<IActionResult> Detail(long id)
        {
            var employee = await mediator.Send(new GetEmployeeByIdQuery(id));
            if (employee == null || employee.Data == null)
                return RedirectToAction("Index");

            return View("Detail", employee.Data);
        }

        [HttpPost]
        [Authorize(Policy = $"Permission:{PermissionValue.Employees.Add}")]
        public async Task<IActionResult> Create([FromBody] CreateEmployeeCommand command)
        {
            var result = await mediator.Send(command);
            return Json(result);
        }

        [HttpPut]
        [Authorize(Policy = $"Permission:{PermissionValue.Employees.Update}")]
        public async Task<IActionResult> Update([FromBody] UpdateEmployeeCommand command)
        {
            var result = await mediator.Send(command);
            return Json(result);
        }

        [HttpGet]
        [Authorize(Policy = $"Permission:{PermissionValue.Employees.Add}")]
        public async Task<IActionResult> GetNextEmployeeNo([FromQuery] bool isIntern)
        {
            var result = await mediator.Send(new GetNextEmployeeNoQuery(isIntern));
            return Json(result);
        }

        [HttpGet]
        [Authorize(Policy = $"Permission:{PermissionValue.Employees.Add}")]
        public async Task<IActionResult> GetNextContractNumber([FromQuery] string contractType)
        {
            var result = await mediator.Send(new GetNextContractNumberQuery(contractType));
            return Json(result);
        }

        [HttpGet]
        [Authorize(Policy = $"Permission:{PermissionValue.Employees.Add}")]
        public async Task<IActionResult> CheckEmployeeNoExists([FromQuery] string employeeNo)
        {
            var result = await mediator.Send(new GetEmployeeNoExistsQuery(employeeNo));
            return Json(result);
        }

        [HttpGet]
        [Authorize(Policy = $"Permission:{PermissionValue.Employees.Add}")]
        public async Task<IActionResult> CheckContractNoExists([FromQuery] string contractNo)
        {
            var result = await mediator.Send(new GetContractNoExistsQuery(contractNo));
            return Json(result);
        }
        [HttpPost("/employees/upload-document")]
        public async Task<IActionResult> UploadDocument(IFormFile file, [FromForm] string employeeNo)
        {
            if (file == null || string.IsNullOrWhiteSpace(employeeNo))
                return BadRequest(new { succeeded = false, messages = new[] { "Missing file or employeeNo" } });

            var result = await mediator.Send(new UploadEmployeeDocumentCommand
            {
                File = file,
                EmployeeNo = employeeNo
            });

            if (result.Succeeded)
                return Ok(new { succeeded = true, data = result.Data, messages = new[] { "Upload successful" } });

            return BadRequest(new { succeeded = false, messages = result.Messages });
        }

        [HttpPost("/employees/delete-document")]
        [Authorize(Policy = $"Permission:{PermissionValue.Employees.Update}")]
        public async Task<IActionResult> DeleteDocument([FromForm] long id)
        {
            var result = await mediator.Send(new DeleteEmployeeDocumentCommand(id));
            return Json(result);
        }

        [HttpPost("/employees/delete")]
        [Authorize(Policy = $"Permission:{PermissionValue.Employees.Delete}")]
        public async Task<IActionResult> Delete([FromForm] long id)
        {
            var result = await mediator.Send(new DeleteEmployeeCommand(id));
            return Json(result);
        }

        [HttpGet("employees/export-contract/{id}")]
        [Authorize(Policy = $"Permission:{PermissionValue.Employees.View}")]
        public async Task<IActionResult> ExportContract(long id)
        {
            var result = await mediator.Send(new ExportEmployeeContractQuery(id));

            if (!result.Succeeded || result.Data == null)
                return NotFound();

            var fileBytes = result.Data.Content;
            var fileName = result.Data.FileName;
            var contentType = result.Data.ContentType;

            return File(fileBytes, contentType, fileName);
        }

        [HttpGet("employees/edit/{id}")]
        [Authorize(Policy = $"Permission:{PermissionValue.Employees.Update}")]
        public async Task<IActionResult> Edit(long id)
        {
            var employee = await mediator.Send(new GetEmployeeByIdQuery(id));
            if (employee == null || employee.Data == null)
                return RedirectToAction("Index");

            return View("Edit", employee.Data);
        }

        [HttpPost("/employees/upload-avatar")]
        public async Task<IActionResult> UploadAvatar(IFormFile file, [FromForm] string employeeNo)
        {
            if (file == null || string.IsNullOrWhiteSpace(employeeNo))
                return BadRequest(new { succeeded = false, messages = new[] { "Missing file or employeeNo" } });

            var result = await mediator.Send(new UploadEmployeeAvatarCommand
            {
                File = file,
                EmployeeNo = employeeNo
            });

            return result.Succeeded
                ? Ok(new { succeeded = true, data = result.Data, messages = new[] { "Upload successful" } })
                : BadRequest(new { succeeded = false, messages = result.Messages });
        }
        [HttpPost("/employees/upload-identity")]
        public async Task<IActionResult> UploadIdentityImage(IFormFile file, [FromForm] string employeeNo, [FromForm] string side)
        {
            if (file == null || string.IsNullOrWhiteSpace(employeeNo) || string.IsNullOrWhiteSpace(side))
                return BadRequest(new { succeeded = false, messages = new[] { "Missing parameters" } });

            var result = await mediator.Send(new UploadEmployeeIdentityCommand
            {
                File = file,
                EmployeeNo = employeeNo,
                Side = side // "front" hoặc "back"
            });

            return result.Succeeded
                ? Ok(new { succeeded = true, data = result.Data, messages = new[] { "Upload successful" } })
                : BadRequest(new { succeeded = false, messages = result.Messages });
        }
        [HttpGet("employees/statistics")]
        [Authorize(Policy = $"Permission:{PermissionValue.Employees.View}")]
        public async Task<IActionResult> GetEmployeeStatistics()
        {
            var result = await mediator.Send(new GetEmployeeStatisticsQuery());
            return Json(result);
        }

    }
}