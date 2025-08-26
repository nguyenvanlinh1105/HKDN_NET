using System.ComponentModel.DataAnnotations;

namespace NineERP.Application.Dtos.Identity.Requests
{
    public class RoleRequest
    {
        public string Id { get; set; } = default!;

        [Required]
        public string Name { get; set; } = default!;

        public string? Description { get; set; }
    }
}