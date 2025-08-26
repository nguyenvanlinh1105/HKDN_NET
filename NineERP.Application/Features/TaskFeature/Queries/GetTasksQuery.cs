using MediatR;
using NineERP.Application.Dtos.Task;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;
using Microsoft.EntityFrameworkCore;

namespace NineERP.Application.Features.TaskFeature.Queries
{
    public record GetTasksQuery : IRequest<GenericResponse<TasksDto>>
    {
        public string? Keyword { get; set; }
        public int ProjectId { get; set; }
        public List<short>? Type { get; set; }
        public List<short>? Status { get; set; }
        public List<short>? Priority { get; set; }
        public List<string>? Assigned { get; set; }
        public List<string>? CreateBy { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsParent { get; set; } = false;
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public class Handler(IApplicationDbContext context) : IRequestHandler<GetTasksQuery, GenericResponse<TasksDto>>
        {
            public async Task<GenericResponse<TasksDto>> Handle(GetTasksQuery request, CancellationToken cancellationToken)
            {
                var query = from t in context.DatTasks.AsNoTracking()
                            where !t.IsDeleted && t.ProjectId == request.ProjectId
                            join mstType in context.MstTypeTasks.AsNoTracking() on t.TypeTaskId equals mstType.Id into mstTypeTemp
                            from type in mstTypeTemp.DefaultIfEmpty()
                            join mstStatus in context.MstStatusTasks.AsNoTracking() on t.StatusTaskId equals mstStatus.Id into mstStatusTemp
                            from status in mstStatusTemp.DefaultIfEmpty()
                            join mstPr in context.MstPriorityTasks on t.PriorityTaskId equals mstPr.Id into mstPriorityTemp
                            from priority in mstPriorityTemp.DefaultIfEmpty()
                            join emp in context.DatEmployees.AsNoTracking() on t.Assigned equals emp.EmployeeNo into empTemp
                            from employee in empTemp.DefaultIfEmpty()
                            where string.IsNullOrEmpty(request.Keyword)
                               || t.TitleTask.Contains(request.Keyword)
                               || t.TaskNo.ToString() == request.Keyword
                            where request.Type == null || request.Type.Contains(t.TypeTaskId)
                            where request.Status == null || request.Status.Contains(t.StatusTaskId)
                            where request.Priority == null || request.Priority.Contains(t.PriorityTaskId)
                            where !request.IsParent || t.ParentId.HasValue
                            where (!request.StartDate.HasValue || request.StartDate.Value.Date <= t.CreatedOn) && (!request.EndDate.HasValue || request.EndDate.Value.Date >= t.CreatedOn)
                            where request.Assigned == null || request.Assigned.Contains(t.Assigned)
                            where request.CreateBy == null || request.CreateBy.Contains(t.CreatedBy)
                            select new TaskDto
                            {
                                Id = t.Id,
                                TitleTask = t.TitleTask,
                                Completed = t.Completed,
                                CreatedOn = t.CreatedOn,
                                TaskNo = t.TaskNo,
                                Duration = t.Duration,
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
                            };

                var totalRecords = await query.CountAsync(cancellationToken);

                var pagedQuery = query
                    .OrderByDescending(t => t.Id)
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize);

                var tasks = await pagedQuery.ToListAsync(cancellationToken);

                var result = new TasksDto
                {
                    Tasks = tasks,
                    CurrentPage = request.PageNumber,
                    TotalPages = (int)Math.Ceiling((double)totalRecords / request.PageSize),
                    TotalCount = totalRecords,
                    PageSize = request.PageSize
                };

                return GenericResponse<TasksDto>.SuccessResponse(200, string.Empty, result);
            }
        }
    }
}
