using NineERP.Domain.Entities.Base;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NineERP.Domain.Entities.Dat
{
    public class DatEmployeeDocumentLog : AuditableBaseEntity<long>
    {
        public long DocumentId { get; set; }

        [MaxLength(50)]
        public string EmployeeNo { get; set; } = default!;

        public string? FileName { get; set; }
        public string? NameFile { get; set; }
        public string? SizeFile { get; set; }
        public string? TypeFile { get; set; }
        public string? GoogleDriveFileId { get; set; }
        public string? GoogleDriveFileUrl { get; set; }

        public string? DeletedBy { get; set; }
        public DateTime? DeletedOn { get; set; }
    }
}
