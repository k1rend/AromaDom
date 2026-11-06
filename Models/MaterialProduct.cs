using System;
using System.Collections.Generic;

namespace AromaDom.Models;

public partial class MaterialProduct
{
    public int MaterialId { get; set; }

    public int ProductId { get; set; }

    public decimal MaterialQuantity { get; set; }

    public virtual Material Material { get; set; } = null!;

    public virtual Product Product { get; set; } = null!;
}
