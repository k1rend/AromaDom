using System;
using System.Collections.Generic;

namespace AromaDom.Models;

public partial class Product
{
    public int ProductId { get; set; }

    public string Article { get; set; } = null!;

    public int ProductTypeId { get; set; }

    public string Name { get; set; } = null!;

    public decimal MinPartnerPrice { get; set; }

    public virtual ICollection<MaterialProduct> MaterialProducts { get; set; } = new List<MaterialProduct>();

    public virtual ProductType ProductType { get; set; } = null!;
}
