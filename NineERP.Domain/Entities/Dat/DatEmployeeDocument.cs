using NineERP.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineERP.Domain.Entities.Dat
{
    public class DatEmployeeDocument : AuditableBaseEntity<long>
    {
        [MaxLength(50)]
        public string EmployeeNo { get; set; } = default!;

        public string? FileName { get; set; }              // Tên gốc
        public string? NameFile { get; set; }              // Tên hiển thị
        public string? SizeFile { get; set; }
        public string? TypeFile { get; set; }

        public string? GoogleDriveFileId { get; set; }     // File ID trên Drive
        public string? GoogleDriveFileUrl { get; set; }    // Link xem
    }
}
