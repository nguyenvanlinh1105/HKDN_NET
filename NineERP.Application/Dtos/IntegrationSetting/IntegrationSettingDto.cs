namespace NineERP.Application.Dtos.IntegrationSetting
{
    public class IntegrationSettingDto
    {
        public string Type { get; set; } = default!;
        public string? ClientId { get; set; }
        public string? ClientSecret { get; set; }
        public string? ServiceAccountJson { get; set; }
        public string? PublicKey { get; set; }
        public string? PrivateKey { get; set; }

        public string? AccessKey { get; set; }
        public string? SecretKey { get; set; }
        public string? Region { get; set; }
        public string? BucketName { get; set; }
        public string? ParentFolderId { get; set; }

        public bool IsActive { get; set; }
    }
}
