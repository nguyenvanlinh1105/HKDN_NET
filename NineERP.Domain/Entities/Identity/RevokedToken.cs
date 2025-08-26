namespace NineERP.Domain.Entities.Identity
{
    public class RevokedToken
    {
        public long Id { get; set; }
        public string Token { get; set; } = default!;
        public DateTime RevokedAt { get; set; }
    }
}
