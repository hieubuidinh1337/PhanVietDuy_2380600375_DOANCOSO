using System;
using System.Collections.Generic;

namespace PhanVietDuy_2380600375.Models.Domain;

public partial class ProductColor
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string ColorName { get; set; } = null!;

    public string? HexCode { get; set; }

    public byte SortOrder { get; set; }

    public virtual Product Product { get; set; } = null!;
}
