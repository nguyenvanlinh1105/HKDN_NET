using DocumentFormat.OpenXml.Spreadsheet;
using GemBox.Spreadsheet;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using NineERP.Application.Dtos.Employees;
using NineERP.Application.Interfaces.Persistence;
using NineERP.Application.Wrapper;

namespace NineERP.Application.Features.EmployeesFeature.Queries;

public record ExportEmployeeContractQuery(long Id) : IRequest<IResult<EmployeeContractFileDto>>
{
    public class Handler(
        IApplicationDbContext context,
        IHostEnvironment environment
    ) : IRequestHandler<ExportEmployeeContractQuery, IResult<EmployeeContractFileDto>>
    {
        public async Task<IResult<EmployeeContractFileDto>> Handle(ExportEmployeeContractQuery request, CancellationToken cancellationToken)
        {
            var employee = await context.DatEmployees
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken);

            if (employee == null)
                return await Result<EmployeeContractFileDto>.FailAsync("Employee not found.");

            var identity = await context.DatEmployeeIdentities
                .AsNoTracking()
                .FirstOrDefaultAsync(x => x.EmployeeNo == employee.EmployeeNo && !x.IsDeleted, cancellationToken);

            var position = await context.MstPositions
                .Where(p => p.Id == employee.PositionId)
                .Select(p => p.NameVi)
                .FirstOrDefaultAsync(cancellationToken);

            var department = await context.MstDepartments
                .Where(d => d.Id == employee.DepartmentId)
                .Select(d => d.NameVi)
                .FirstOrDefaultAsync(cancellationToken);

            var contractTypeName = string.Empty;
            if (employee.ContractTypeId.HasValue)
            {
                contractTypeName = await context.KbnContractTypes
                    .Where(c => c.Id == employee.ContractTypeId.Value)
                    .Select(c => c.NameVi)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            var positionName = string.Empty;
            if (employee.PositionId.HasValue)
            {
                positionName = await context.MstPositions
                    .Where(p => p.Id == employee.PositionId.Value)
                    .Select(p => p.NameVi)
                    .FirstOrDefaultAsync(cancellationToken);
            }

            // 🔹 Salary Info
            var salary = await context.DatEmployeeSalaries.AsNoTracking()
                .FirstOrDefaultAsync(x => x.EmployeeNo == employee.EmployeeNo && !x.IsDeleted, cancellationToken);

            SpreadsheetInfo.SetLicense("FREE-LIMITED-KEY");
            var workbook = new ExcelFile();
            var ws = workbook.Worksheets.Add("Hợp đồng nhân viên");
            var contractDay = DateTime.Now.ToString("dd/MM/yyyy");
            ws.Cells.Style.Font.Name = "Times New Roman";

            ws.Cells[0, 2].Value = "CỘNG HÒA XÃ HỘI CHỦ NGHĨA VIỆT NAM";
            ws.Cells[0, 2].Style.Font.Weight = ExcelFont.BoldWeight;
            ws.Cells[0, 2].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            ws.Cells.GetSubrangeAbsolute(0, 2, 0, 6).Merged = true;

            ws.Cells[1, 2].Value = "Độc lập - Tự do - Hạnh phúc";
            ws.Cells[1, 2].Style.Font.Weight = ExcelFont.BoldWeight;
            ws.Cells[1, 2].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            ws.Cells.GetSubrangeAbsolute(1, 2, 1, 6).Merged = true;

            ws.Cells[2, 2].Value = "***********";
            ws.Cells[2, 2].Style.Font.Weight = ExcelFont.BoldWeight;
            ws.Cells[2, 2].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            ws.Cells.GetSubrangeAbsolute(2, 2, 2, 6).Merged = true;

            ws.Cells[3, 6].Value = $"Đà Nẵng, ngày {contractDay}";

            ws.Cells[5, 2].Value = "HỢP ĐỒNG LAO ĐỘNG";
            ws.Cells[5, 2].Style.Font.Weight = ExcelFont.BoldWeight;
            ws.Cells[5, 2].Style.Font.Size = 360;
            ws.Cells[5, 2].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            ws.Cells.GetSubrangeAbsolute(5, 2, 5, 6).Merged = true;

            ws.Cells[6, 2].Value = $"Số: {employee.ContractNumber}/HĐLĐ-NINEPLUS";
            ws.Cells[6, 2].Style.Font.Weight = ExcelFont.BoldWeight;
            ws.Cells[6, 2].Style.Font.Size = 360;
            ws.Cells[6, 2].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;
            ws.Cells.GetSubrangeAbsolute(6, 2, 6, 6).Merged = true;

            ws.Cells[7, 1].Value = $"(Ban hành theo Thông tư số 10/2020/TT-BLĐTBXH ngày 12/11/2020 của Bộ Lao động - Thương binh và Xã hội)";
            ws.Cells.GetSubrangeAbsolute(7, 1, 8, 8).Merged = true;
            ws.Cells[7, 1].Style.WrapText = true;
            ws.Cells[7, 1].Style.HorizontalAlignment = HorizontalAlignmentStyle.Center;

            ws.Cells[9, 0].Value = $"Chúng tôi, một bên là Ông/Bà: DƯƠNG CHÂU VĨNH PHÚC";
            ws.Cells[9, 0].Style.Font.Weight = ExcelFont.BoldWeight;

            ws.Cells[10, 7].Value = $"Quốc tịch: ";
            ws.Cells[10, 8].Value = $"Việt Nam";

            ws.Cells[11, 0].Value = $"Chức vụ: ";
            ws.Cells[11, 2].Value = $"Giám đốc";
            ws.Cells[11, 2].Style.Font.Weight = ExcelFont.BoldWeight;
            ws.Cells[11, 7].Value = $"Điện thoại: ";
            ws.Cells[11, 8].Value = $"0944 99 25 11";

            ws.Cells[12, 0].Value = $"Sinh ngày: ";
            ws.Cells[12, 2].Value = $"25/11/1988";
            ws.Cells[12, 7].Value = $"Tại: ";
            ws.Cells[12, 8].Value = $"Quảng Nam";

            ws.Cells[13, 0].Value = $"Số CCCD: ";
            ws.Cells[13, 2].Value = $"049088003561";
            ws.Cells[13, 4].Value = $"Cấp ngày: ";
            ws.Cells[13, 5].Value = $"25/01/2022";
            ws.Cells[13, 7].Value = $"Tại: ";
            ws.Cells[13, 8].Value = $"Cục CS về QLHC & TTXH";
            ws.Cells.GetSubrangeAbsolute(13, 8, 14, 9).Merged = true;
            ws.Cells[13, 8].Style.WrapText = true;

            ws.Cells[15, 0].Value = $"Đại diện cho: ";
            ws.Cells[15, 0].Style.Font.Weight = ExcelFont.BoldWeight;
            ws.Cells[15, 2].Value = $"CÔNG TY CỔ PHẦN ĐẦU TƯ VÀ PHÁT TRIỂN GIẢI PHÁP CÔNG NGHỆ NINEPLUS";
            ws.Cells[15, 2].Style.Font.Weight = ExcelFont.BoldWeight;
            ws.Cells.GetSubrangeAbsolute(15, 2, 16, 9).Merged = true;
            ws.Cells[15, 2].Style.WrapText = true;

            ws.Cells[17, 0].Value = $"Địa chỉ: ";
            ws.Cells[17, 2].Value = $"193 Xô Viết Nghệ Tĩnh, P.Khuê Trung, Q. Cẩm Lệ, TP. Đà Nẵng";

            ws.Cells[19, 0].Value = $"Và một bên là Ông/Bà: {employee.FullName}";
            ws.Cells[19, 0].Style.Font.Weight = ExcelFont.BoldWeight;

            ws.Cells[20, 0].Value = $"Điện thoại: ";
            ws.Cells[20, 2].Value = $"{employee.PhoneNo}";
            ws.Cells[20, 7].Value = $"Quốc tịch: ";
            ws.Cells[20, 8].Value = $"Việt Nam";

            ws.Cells[21, 0].Value = $"Sinh ngày: ";
            ws.Cells[21, 2].Value = $"{employee.Birthday:dd/MM/yyyy}";
            ws.Cells[21, 7].Value = $"Tại: ";
            ws.Cells[21, 8].Value = $"{employee.PlaceOfBirth}";

            ws.Cells[22, 0].Value = $"Số CCCD: ";
            ws.Cells[22, 2].Value = $"{identity.CitizenshipCard}";
            ws.Cells[22, 4].Value = $"Cấp ngày: ";
            ws.Cells[22, 5].Value = $"{identity.ProvideDateCitizenshipCard:dd/MM/yyyy}";
            ws.Cells[22, 7].Value = $"Tại: ";
            ws.Cells[22, 8].Value = $"{identity.ProvidePlaceCitizenshipCard}";
            ws.Cells.GetSubrangeAbsolute(22, 8, 23, 9).Merged = true;
            ws.Cells[22, 8].Style.WrapText = true;

            ws.Cells[24, 0].Value = $"Địa chỉ: ";
            ws.Cells[24, 2].Value = $"{employee.Address}";

            ws.Cells[26, 0].Value = "Thỏa thuận ký kết hợp đồng lao động và cam kết làm đúng theo những điều khoản sau đây:";

            ws.Cells[28, 0].Value = "ĐIỀU 1: THỜI HẠN VÀ CÔNG VIỆC HỢP ĐỒNG";
            ws.Cells[28, 0].Style.Font.Weight = ExcelFont.BoldWeight;

            ws.Cells[30, 0].Value = $"Loại hợp đồng lao động: ";
            ws.Cells[30, 3].Value = $"{contractTypeName}";

            ws.Cells[31, 0].Value = $"Thời hạn hợp đồng: ";
            ws.Cells[31, 3].Value = $"Từ {employee.ContractFrom:dd/MM/yyyy} đến hết {employee.ContractTo:dd/MM/yyyy}";

            ws.Cells[32, 0].Value = $"Địa điểm làm việc: ";
            ws.Cells[32, 3].Value = $"Tại Văn phòng Công ty Cổ phần Đầu tư và Phát triển giải pháp Công nghệ Nine Plus";
            ws.Cells.GetSubrangeAbsolute(32, 3, 33, 9).Merged = true;
            ws.Cells[32, 3].Style.WrapText = true;

            ws.Cells[34, 0].Value = $"Chức danh chuyên môn: ";
            ws.Cells[34, 3].Value = $"{positionName}";

            ws.Cells[35, 0].Value = $"Công việc phải làm: ";
            ws.Cells[35, 3].Value = $"Thực hiện các công việc phát triển kinh doanh của công ty, theo sự chỉ đạo của cấp trên";
            ws.Cells.GetSubrangeAbsolute(35, 3, 36, 9).Merged = true;
            ws.Cells[35, 3].Style.WrapText = true;

            ws.Cells[37, 0].Value = "ĐIỀU 2: CHẾ ĐỘ LÀM VIỆC";
            ws.Cells[37, 0].Style.Font.Weight = ExcelFont.BoldWeight;

            ws.Cells[39, 0].Value = $"Thời gian làm việc: ";
            ws.Cells[39, 3].Value = $"08 giờ/ngày, từ 8:30 đến 17:30";
            ws.Cells[40, 3].Value = $"05 ngày/ tuần, từ thứ 2 đến thứ 6";
            ws.Cells[41, 3].Value = $"Nghỉ giao lao: từ 12:00 đến 13:00";
            ws.Cells[42, 3].Value = $"Nghỉ hằng tuần: thứ 7, Chủ nhật";


            ws.Cells[44, 0].Value = "ĐIỀU 3: NGHĨA VỤ VÀ QUYỀN LỢI CỦA NGƯỜI LAO ĐỘNG";
            ws.Cells[44, 0].Style.Font.Weight = ExcelFont.BoldWeight;

            ws.Cells[46, 0].Value = $"Quyền lợi: ";
            ws.Cells[46, 0].Style.Font.Weight = ExcelFont.BoldWeight;

            ws.Cells[47, 0].Value = $"Phương tiện đi lại làm việc: ";
            ws.Cells[47, 3].Value = $"Tự túc";

            ws.Cells[48, 0].Value = $"Tiền lương cơ bản: ";
            ws.Cells[48, 3].Value = $"{salary?.SalaryBasic}";

            ws.Cells[49, 0].Value = $"Trợ cấp làm thêm: ";
            ws.Cells[49, 3].Value = $"Căn cứ theo quy định của pháp luật";

            
            var outputDir = Path.Combine(environment.ContentRootPath, "wwwroot", "export-files");
            Directory.CreateDirectory(outputDir);

            var fileName = $"HĐLĐ-NINEPLUS-{employee.FullName}-{employee.EmployeeNo}.pdf";
            var outputPath = Path.Combine(outputDir, fileName);
            workbook.Save(outputPath);

            byte[] fileBytes = await File.ReadAllBytesAsync(outputPath, cancellationToken);
            File.Delete(outputPath);

            var fileDto = new EmployeeContractFileDto
            {
                FileName = fileName,
                ContentType = "application/pdf",
                Content = fileBytes
            };

            return await Result<EmployeeContractFileDto>.SuccessAsync(fileDto);
        }
    }
}
