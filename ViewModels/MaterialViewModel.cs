namespace AromaDom.ViewModels;


public class MaterialViewModel
{
    public int MaterialId { get; set; }

    public string TypeName { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public decimal MinQuantity { get; set; }

    public decimal StockQuantity { get; set; }

    public decimal Price { get; set; }

    public string Unit { get; set; } = string.Empty;


    public decimal RequiredQuantity { get; set; }
}
