using System;
using System.Collections.Generic;

namespace PhanVietDuy_2380600375.Models.Domain;

public partial class ApiLog
{
    public long Id { get; set; }

    public string Endpoint { get; set; } = null!;

    public string Method { get; set; } = null!;

    public short StatusCode { get; set; }

    public string? RequestBody { get; set; }

    public string? UserId { get; set; }

    public string? IpAddress { get; set; }

    public int? DurationMs { get; set; }

    public DateTime CreatedAt { get; set; }
}
