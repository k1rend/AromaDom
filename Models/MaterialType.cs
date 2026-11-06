using System;
using System.Collections.Generic;

namespace AromaDom.Models;

public partial class MaterialType
{
    public int MaterialTypeId { get; set; }

    public string Name { get; set; } = null!;

    public decimal LossPercent { get; set; }

    public virtual ICollection<Material> Materials { get; set; } = new List<Material>();
}
