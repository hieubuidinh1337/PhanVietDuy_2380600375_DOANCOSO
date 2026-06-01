using System;
using System.Collections.Generic;

namespace PhanVietDuy_2380600375.Models.Domain;

public partial class ProductSize
{
    public int Id { get; set; }

    public int ProductId { get; set; }

    public string SizeName { get; set; } = null!;

    public byte SortOrder { get; set; }

    public virtual Product Product { get; set; } = null!;
}
