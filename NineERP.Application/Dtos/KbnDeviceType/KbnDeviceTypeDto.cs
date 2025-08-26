namespace NineERP.Application.Dtos.KbnDeviceType;

public class KbnDeviceTypeDto
{
    public int Id { get; set; }
    public string NameVi { get; set; } = default!;
    public string NameEn { get; set; } = default!;
    public string NameJp { get; set; } = default!;
    public string? Description { get; set; }
}
