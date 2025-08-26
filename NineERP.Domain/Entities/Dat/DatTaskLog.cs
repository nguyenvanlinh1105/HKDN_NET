using NineERP.Domain.Entities.Base;

namespace NineERP.Domain.Entities.Dat
{
    public class DatTaskLog : AuditableBaseEntity<int>
    {
        public int TaskId { get; set; }
        public short Action { get; set; } // 1 - Thêm mới / 2- Update/ 3-Xóa
        public string TableName { get; set; } = default!;
        public string? DataOld { get; set; }
        public string? DataNew { get; set; }
    }
}
