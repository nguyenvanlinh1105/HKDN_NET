using NineERP.Domain.Entities.Base;
using NineERP.Domain.Enums;

namespace NineERP.Domain.Entities
{
    public class IntegrationSetting : AuditableBaseEntity<int>
    {
        public IntegrationServiceType Type { get; set; } // Enum: Drive, Calendar, reCAPTCHA, S3

        // Common fields
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? PublicKey { get; set; }     // reCAPTCHA
        public string? PrivateKey { get; set; }

        // Google Service Account
        public string? ServiceAccountJson { get; set; }

        // S3 Specific
        public string? AccessKey { get; set; }
        public string? SecretKey { get; set; }
        public string? Region { get; set; }
        public string? BucketName { get; set; }
        public string? ParentFolderId { get; set; }
        public bool IsActive { get; set; }
    }
}
