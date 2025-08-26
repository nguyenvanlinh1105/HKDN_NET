namespace NineERP.Application.Dtos.Identity.Requests
{
    public class ToggleUserStatusRequest
    {
        public bool ActivateUser { get; set; }
        public string UserId { get; set; } = default!;
    }
}