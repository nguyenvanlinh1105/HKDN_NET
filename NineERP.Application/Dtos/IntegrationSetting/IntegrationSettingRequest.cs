namespace NineERP.Application.Dtos.IntegrationSetting
{
    public class IntegrationSettingRequest
    {
        public string Type { get; set; } = default!;
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? ServiceAccountJson { get; set; }
        public string? PublicKey { get; set; }
        public string? PrivateKey { get; set; }

        public string? AccessKey { get; set; }      // S3
        public string? SecretKey { get; set; }      // S3
        public string? Region { get; set; }         // S3
        public string? BucketName { get; set; }     // S3
        public string? ParentFolderId { get; set; } 

        public bool IsActive { get; set; }
    }
}
