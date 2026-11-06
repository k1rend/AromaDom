using AromaDom.Data;
using Microsoft.EntityFrameworkCore;

namespace AromaDom.Services;


public class ProductionCalculator
{
    private readonly AromaDomDevContext _context;

    public ProductionCalculator(AromaDomDevContext context)
    {
        _context = context;
    }


    public async Task<int> Calculate(
        int productTypeId,
        int materialTypeId,
        int rawQuantity,
        double parameter1,
        double parameter2)
    {
        if (rawQuantity <= 0 ||
            parameter1 <= 0 ||
            parameter2 <= 0 ||
            double.IsNaN(parameter1) ||
            double.IsNaN(parameter2) ||
            double.IsInfinity(parameter1) ||
            double.IsInfinity(parameter2))
        {
            return -1;
        }

        var productType = await _context.ProductTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.ProductTypeId == productTypeId);

        var materialType = await _context.MaterialTypes
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.MaterialTypeId == materialTypeId);

        if (productType is null || materialType is null)
        {
            return -1;
        }

        double coefficient = (double)productType.Coefficient;
        double lossPercent = (double)materialType.LossPercent;

        if (coefficient <= 0 || lossPercent < 0 || lossPercent > 100)
        {
            return -1;
        }

        
        double rawPerUnit = parameter1 * parameter2 * coefficient;

        if (rawPerUnit <= 0)
        {
            return -1;
        }

        
        double usableRaw = rawQuantity * (1 - lossPercent / 100.0);
        double productsCount = usableRaw / rawPerUnit;

        if (double.IsNaN(productsCount) ||
            double.IsInfinity(productsCount) ||
            productsCount < 0 ||
            productsCount > int.MaxValue)
        {
            return -1;
        }

        
        return (int)Math.Floor(productsCount);
    }
}
