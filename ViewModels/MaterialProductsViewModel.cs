namespace AromaDom.ViewModels;


public class MaterialProductsViewModel
{
    public int MaterialId { get; set; }

    public string MaterialName { get; set; } = string.Empty;

    public string Unit { get; set; } = string.Empty;

    public List<ProductUsageViewModel> Products { get; set; } = new();

    public decimal TotalQuantity => Products.Sum(p => p.RequiredQuantity);
}


public class ProductUsageViewModel
{
    public int ProductId { get; set; }

    public string ProductName { get; set; } = string.Empty;

    public string Article { get; set; } = string.Empty;

    public decimal RequiredQuantity { get; set; }
}
