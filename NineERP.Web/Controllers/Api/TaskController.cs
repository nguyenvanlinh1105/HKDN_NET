using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NineERP.Application.Dtos.Task;
using NineERP.Application.Features.TaskFeature.Commands;
using NineERP.Application.Features.TaskFeature.Queries;
using NineERP.Application.Wrapper;

namespace NineERP.Web.Controllers.Api
{
    [ApiController]
    [Route("api/tasks")]
    [ApiExplorerSettings(GroupName = "v1")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TaskController(IMediator mediator, IValidator<AddCommentTaskDto> validator) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetTasks([FromQuery] GetTasksQuery request)
        {
            var response = await mediator.Send(request);
            return new ObjectResult(response) { StatusCode = response.Status };
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetTask(int id)
        {
            var response = await mediator.Send(new GetTaskQuery { Id = id });
            return new ObjectResult(response) { StatusCode = response.Status };
        }

        [HttpGet("mine")]
        public async Task<IActionResult> GetTasksCurrentUser([FromQuery] GetTasksCurrentUserQuery request)
        {
            var response = await mediator.Send(request);
            return new ObjectResult(response) { StatusCode = response.Status };
        }

        [HttpGet("comments")]
        public async Task<IActionResult> GetCommentsTask([FromQuery] GetCommentsQuery request)
        {
            var response = await mediator.Send(request);
            return new ObjectResult(response) { StatusCode = response.Status };
        }

        [HttpPost("comments")]
        public async Task<IActionResult> AddCommentsTask([FromBody] AddCommentTaskDto request)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                var errors = validationResult.Errors
                    .Select(e => new ErrorDetail
                    {
                        Code = e.ErrorCode,
                        Details = e.ErrorMessage
                    })
                    .ToList();

                return new ObjectResult(GenericResponse<object>.MultipleErrorsResponse(400, "", errors)) { StatusCode = 400 };
            }
            var response = await mediator.Send(new AddCommentCommand()
            {
                TaskId = request.TaskId,
                Comment = request.Comment
            });

            return new ObjectResult(response) { StatusCode = response.Status };
        }

        [HttpPost("comments/delete")]
        public async Task<IActionResult> AddCommentsTask([FromBody] DeleteCommentTaskCommand request)
        {
            var response = await mediator.Send(request);
            return new ObjectResult(response) { StatusCode = response.Status };
        }

        [HttpGet("subtasks")]
        public async Task<IActionResult> GetSubTasks([FromQuery] GetSubTasksQuery request)
        {
            var response = await mediator.Send(request);
            return new ObjectResult(response) { StatusCode = response.Status };
        }

        [HttpGet("documents")]
        public async Task<IActionResult> GetDocumentsTasks([FromQuery] GetDocumentsTaskQuery request)
        {
            var response = await mediator.Send(request);
            return new ObjectResult(response) { StatusCode = response.Status };
        }
    }
}
