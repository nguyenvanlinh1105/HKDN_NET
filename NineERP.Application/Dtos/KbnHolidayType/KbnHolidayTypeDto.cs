namespace NineERP.Application.Dtos.KbnHolidayType;

public class KbnHolidayTypeDto
{
    public int Id { get; set; }
    public string NameVi { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public string NameJp { get; set; } = default!;
    public string? Description { get; set; }
}
