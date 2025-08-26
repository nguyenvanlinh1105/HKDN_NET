using NineERP.Application.Wrapper;

namespace NineERP.Application.Dtos.Project
{
    public class ProjectsDto : PaginatedResultApi
    {
        public List<ProjectDto>? Projects { get; set; }

    }

    public class ProjectDto
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public short ProjectType { get; set; } // 1: Internal, 2: External, 3: Fix price
        public short StatusProject { get; set; } // 1: New, 2: In progress, 3: Completed, 4: Cancelled, 5: Closed
        public int TotalParticipants { get; set; }
        public List<string?>? ImageTopThreeParticipants { get; set; }
    }

    public class ProjectDetailDto
    {
        public string? Name { get; set; }
        public short ProjectType { get; set; } // 1: Internal, 2: External, 3: Fix price
        public short StatusProject { get; set; } // 1: New, 2: In progress, 3: Completed, 4: Cancelled, 5: Closed
        public int Duration { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? CustomerName { get; set; }
        public string? Technology { get; set; }
        public string? Note { get; set; }
        public string? ImageUrl { get; set; }
    }

    public class ParticipantsDto : PaginatedResultApi
    {
        public List<ParticipantDto>? Participants { get; set; }
    }

    public class ParticipantDto
    {
        public string? EmployeeNo { get; set; }
        public string? NickName { get; set; }
        public string? RoleNameVi { get; set; }
        public string? RoleNameEn { get; set; }
        public string? RoleNameJa { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime JoinDate { get; set; }
        public short Status { get; set; } // 0: Doing, 1: Finish
    }
}
