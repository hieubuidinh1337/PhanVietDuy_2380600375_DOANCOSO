using Microsoft.AspNetCore.Identity;
using System.Collections.Generic;

namespace PhanVietDuy_2380600375.Models.Domain
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public byte? Age { get; set; }
        public string? AvatarUrl { get; set; }
        public bool IsActive { get; set; } = true;
        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? LastLoginAt { get; set; }

        public virtual ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
        public virtual ICollection<Order> OrderProcessedByAdmins { get; set; } = new List<Order>();
        public virtual ICollection<Order> OrderUsers { get; set; } = new List<Order>();
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
        public virtual ICollection<Review> Reviews { get; set; } = new List<Review>();
        public virtual ICollection<ContactMessage> ContactMessages { get; set; } = new List<ContactMessage>();
    }
}
