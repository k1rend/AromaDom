using System;
using System.Collections.Generic;

namespace AromaDom.Models;

public partial class Material
{
    public int MaterialId { get; set; }

    public int MaterialTypeId { get; set; }

    public string Name { get; set; } = null!;

    public decimal Price { get; set; }

    public decimal StockQuantity { get; set; }

    public decimal MinQuantity { get; set; }

    public decimal PackageQuantity { get; set; }

    public string Unit { get; set; } = null!;

    public virtual ICollection<MaterialProduct> MaterialProducts { get; set; } = new List<MaterialProduct>();

    public virtual MaterialType MaterialType { get; set; } = null!;
}
