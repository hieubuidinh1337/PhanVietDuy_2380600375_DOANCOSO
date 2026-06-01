using System;
using System.Collections.Generic;

namespace PhanVietDuy_2380600375.Models.Domain;

public partial class NewsletterSubscriber
{
    public int Id { get; set; }

    public string Email { get; set; } = null!;

    public string? FullName { get; set; }

    public bool IsActive { get; set; }

    public DateTime SubscribedAt { get; set; }

    public DateTime? UnsubscribedAt { get; set; }

    public string? ConfirmToken { get; set; }

    public bool IsConfirmed { get; set; }
}
