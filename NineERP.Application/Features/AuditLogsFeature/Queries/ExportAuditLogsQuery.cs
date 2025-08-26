using ClosedXML.Excel;
using MediatR;
using Microsoft.EntityFrameworkCore;
using NineERP.Application.Dtos.AuditLog;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Request;
using System.IO;

namespace NineERP.Application.Features.AuditLogsFeature.Queries;

public record ExportAuditLogsQuery(AuditLogRequest Request) : IRequest<byte[]>;

public class ExportAuditLogsQueryHandler : IRequestHandler<ExportAuditLogsQuery, byte[]>
{
    private readonly IApplicationDbContext _context;

    public ExportAuditLogsQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<byte[]> Handle(ExportAuditLogsQuery request, CancellationToken cancellationToken)
    {
        var keyword = request.Request.Keyword?.ToLower();

        var query = _context.AuditLogs.AsNoTracking()
            .Where(x => !x.IsDeleted &&
                        (string.IsNullOrEmpty(keyword)
                         || x.TableName.ToLower().Contains(keyword)
                         || x.ActionType.ToLower().Contains(keyword)
                         || x.UserName!.ToLower().Contains(keyword)));

        var list = await query.OrderByDescending(x => x.ActionTimestamp).ToListAsync(cancellationToken);

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("AuditLogs");

        // Headers
        worksheet.Cell(1, 1).Value = "User";
        worksheet.Cell(1, 2).Value = "Table";
        worksheet.Cell(1, 3).Value = "Action";
        worksheet.Cell(1, 4).Value = "IP Address";
        worksheet.Cell(1, 5).Value = "Time";
        worksheet.Cell(1, 6).Value = "Old Values";
        worksheet.Cell(1, 7).Value = "New Values";

        // Content
        for (int i = 0; i < list.Count; i++)
        {
            var row = i + 2;
            var log = list[i];
            worksheet.Cell(row, 1).Value = log.UserName;
            worksheet.Cell(row, 2).Value = log.TableName;
            worksheet.Cell(row, 3).Value = log.ActionType;
            worksheet.Cell(row, 4).Value = log.IpAddress;
            worksheet.Cell(row, 5).Value = log.ActionTimestamp.ToString("yyyy-MM-dd HH:mm:ss");
            worksheet.Cell(row, 6).Value = log.OldValues;
            worksheet.Cell(row, 7).Value = log.NewValues;
        }

        // Auto adjust column width
        worksheet.Columns().AdjustToContents();

        // Export to byte array
        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
}
