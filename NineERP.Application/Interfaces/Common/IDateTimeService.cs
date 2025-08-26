namespace NineERP.Application.Interfaces.Common
{
    public interface IDateTimeService
    {
        DateTime Now { get; }
        TimeZoneInfo TimeZoneInfo { get; }
    }
}
