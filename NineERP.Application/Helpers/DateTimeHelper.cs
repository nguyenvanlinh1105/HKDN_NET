using AutoMapper;
using System.Globalization;

namespace NineERP.Application.Helpers
{
    public class DateTimeToStringConverter : ITypeConverter<DateTime, string>
    {
        public string Convert(DateTime source, string destination, ResolutionContext context)
        {
            return source.ToString("dd/MM/yyyy", CultureInfo.InvariantCulture);
        }
    }
    public class StringToDateTimeConverter : ITypeConverter<string, DateTime>
    {
        public DateTime Convert(string source, DateTime destination, ResolutionContext context)
        {
            if (DateTime.TryParseExact(source, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            {
                return result;
            }

            throw new FormatException("Invalid date format.");
        }
    }
}
