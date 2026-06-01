using Microsoft.AspNetCore.Identity;

namespace PhanVietDuy_2380600375.Models.Domain
{
    public class ApplicationRole : IdentityRole
    {
        public string? Description { get; set; }
    }
}
