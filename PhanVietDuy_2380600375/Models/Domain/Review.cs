using System;
using System.Collections.Generic;

namespace PhanVietDuy_2380600375.Models.Domain;

public partial class Review
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string? UserId { get; set; }

    public string AuthorName { get; set; } = null!;

    public string? AuthorTitle { get; set; }

    public byte Rating { get; set; }

    public string Content { get; set; } = null!;

    public bool IsApproved { get; set; }

    public bool IsHomepage { get; set; }

    public DateTime CreatedAt { get; set; }

    public virtual Product Product { get; set; } = null!;

    public virtual ApplicationUser? User { get; set; }
}
