using NineERP.Application.Wrapper;

namespace NineERP.Application.Dtos.Task
{
    public class TasksDto : PaginatedResultApi
    {
        public List<TaskDto>? Tasks { get; set; }
    }

    public class TaskDto
    {
        public int Id { get; set; }
        public string? TypeTaskVi { get; set; }
        public string? TypeTaskEn { get; set; }
        public string? TypeTaskJa { get; set; }
        public int TaskNo { get; set; }
        public string TitleTask { get; set; } = default!;
        public string? PriorityTaskVi { get; set; }
        public string? PriorityTaskEn { get; set; }
        public string? PriorityTaskJa { get; set; }
        public string? PriorityTaskColor { get; set; }
        public decimal? Duration { get; set; }
        public string? StatusTaskVi { get; set; }
        public string? StatusTaskEn { get; set; }
        public string? StatusTaskJa { get; set; }
        public string? StatusTaskColor { get; set; }
        public short? Completed { get; set; }
        public string? UrlImageAssigned { get; set; }
        public DateTime CreatedOn { get; set; }
    }

    public class TaskDetailDto
    {
        public string? TypeTaskVi { get; set; }
        public string? TypeTaskEn { get; set; }
        public string? TypeTaskJa { get; set; }
        public int TaskNo { get; set; }
        public string TitleTask { get; set; } = default!;
        public string? PriorityTaskVi { get; set; }
        public string? PriorityTaskEn { get; set; }
        public string? PriorityTaskJa { get; set; }
        public string? PriorityTaskColor { get; set; }
        public decimal? Duration { get; set; }
        public decimal? Estimate { get; set; }
        public string? StatusTaskVi { get; set; }
        public string? StatusTaskEn { get; set; }
        public string? StatusTaskJa { get; set; }
        public string? StatusTaskColor { get; set; }
        public short? Completed { get; set; }
        public string? UrlImageAssigned { get; set; }
        public string? AssignedEmployeeNo { get; set; }
        public string? AssignedNickName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedOn { get; set; }
        public int? ParentTaskId { get; set; }
        public ParentTaskDto? ParentTask { get; set; }
    }

    public class ParentTaskDto
    {
        public int Id { get; set; }
        public int TaskNo { get; set; }
        public string TitleTask { get; set; } = default!;
        public string? TypeTaskVi { get; set; }
        public string? TypeTaskEn { get; set; }
        public string? TypeTaskJa { get; set; }
    }
}
