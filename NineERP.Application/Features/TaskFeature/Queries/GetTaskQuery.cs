using MediatR;
using NineERP.Application.Dtos.Task;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Constants.Messages;

namespace NineERP.Application.Features.TaskFeature.Queries
{
    public record GetTaskQuery : IRequest<GenericResponse<TaskDetailDto>>
    {
        public int Id { get; set; }
        public class Handler(IApplicationDbContext context) : IRequestHandler<GetTaskQuery, GenericResponse<TaskDetailDto>>
        {
            public async Task<GenericResponse<TaskDetailDto>> Handle(GetTaskQuery request, CancellationToken cancellationToken)
            {
                var query = from t in context.DatTasks.AsNoTracking()
                            where !t.IsDeleted && t.Id == request.Id
                            join mstType in context.MstTypeTasks.AsNoTracking() on t.TypeTaskId equals mstType.Id into mstTypeTemp
                            from type in mstTypeTemp.DefaultIfEmpty()
                            join mstStatus in context.MstStatusTasks.AsNoTracking() on t.StatusTaskId equals mstStatus.Id into mstStatusTemp
                            from status in mstStatusTemp.DefaultIfEmpty()
                            join mstPr in context.MstPriorityTasks on t.PriorityTaskId equals mstPr.Id into mstPriorityTemp
                            from priority in mstPriorityTemp.DefaultIfEmpty()
                            join emp in context.DatEmployees.AsNoTracking() on t.Assigned equals emp.EmployeeNo into empTemp
                            from employee in empTemp.DefaultIfEmpty()
                            select new TaskDetailDto
                            {
                                TitleTask = t.TitleTask,
                                Completed = t.Completed,
                                CreatedOn = t.CreatedOn,
                                TaskNo = t.TaskNo,
                                Duration = t.Duration,
                                Estimate = t.Estimate,
                                StartDate = t.StartDate,
                                EndDate = t.EndDate,
                                Description = t.Description,
                                TypeTaskEn = type != null ? type.NameEn : string.Empty,
                                TypeTaskVi = type != null ? type.NameVi : string.Empty,
                                TypeTaskJa = type != null ? type.NameJa : string.Empty,
                                StatusTaskEn = status != null ? status.NameEn : string.Empty,
                                StatusTaskVi = status != null ? status.NameVi : string.Empty,
                                StatusTaskJa = status != null ? status.NameJa : string.Empty,
                                StatusTaskColor = status != null ? status.Color : string.Empty,
                                PriorityTaskEn = priority != null ? priority.NameEn : string.Empty,
                                PriorityTaskVi = priority != null ? priority.NameVi : string.Empty,
                                PriorityTaskJa = priority != null ? priority.NameJa : string.Empty,
                                PriorityTaskColor = priority != null ? priority.Color : string.Empty,
                                UrlImageAssigned = employee != null ? employee.ImageURL : string.Empty,
                                AssignedEmployeeNo = employee != null ? employee.EmployeeNo : string.Empty,
                                AssignedNickName = employee != null ? employee.NickName : string.Empty,
                                ParentTaskId = t.ParentId
                            };
                var result = await query.FirstOrDefaultAsync(cancellationToken);
                if (result == null) return GenericResponse<TaskDetailDto>.ErrorResponse(400, ErrorMessages.GetMessage("T001"), "T001", ErrorMessages.GetMessage("T001"));

                if (result.ParentTaskId.HasValue)
                {
                    var parentTask = from t in context.DatTasks.AsNoTracking()
                                     where !t.IsDeleted && t.Id == result.ParentTaskId.Value
                                     join mstType in context.MstTypeTasks.AsNoTracking() on t.TypeTaskId equals mstType.Id into mstTypeTemp
                                     from type in mstTypeTemp.DefaultIfEmpty()
                                     select new ParentTaskDto
                                     {
                                         Id = t.Id,
                                         TitleTask = t.TitleTask,
                                         TaskNo = t.TaskNo,
                                         TypeTaskEn = type != null ? type.NameEn : string.Empty,
                                         TypeTaskVi = type != null ? type.NameVi : string.Empty,
                                         TypeTaskJa = type != null ? type.NameJa : string.Empty,
                                     };

                    result.ParentTask = await parentTask.FirstOrDefaultAsync(cancellationToken);
                }

                return GenericResponse<TaskDetailDto>.SuccessResponse(200, string.Empty, result);
            }
        }
    }
}
