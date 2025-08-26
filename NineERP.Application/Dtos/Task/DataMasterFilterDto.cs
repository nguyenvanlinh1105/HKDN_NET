namespace NineERP.Application.Dtos.Task
{
    public class DataMasterFilterDto
    {
        public List<DataFilterDto>? Types { get; set; }
        public List<DataFilterDto>? Status { get; set; }
        public List<DataFilterDto>? Priorities { get; set; }
        public List<DataFilterUserDto>? Users { get; set; }
    }

    public class DataFilterDto
    {
        public short Id { get; set; }
        public string NameVi { get; set; } = default!;
        public string NameJa { get; set; } = default!;
        public string NameEn { get; set; } = default!;
    }

    public class DataFilterUserDto
    {
        public string EmployeeNo { get; set; } = default!;
        public string NickName { get; set; } = default!;
    }
}
