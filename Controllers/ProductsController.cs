using AromaDom.Data;
using AromaDom.Services;
using AromaDom.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AromaDom.Controllers;


public class ProductsController : Controller
{
    private readonly AromaDomDevContext _context;
    private readonly ProductionCalculator _calculator;

    public ProductsController(AromaDomDevContext context, ProductionCalculator calculator)
    {
        _context = context;
        _calculator = calculator;
    }

    // GET: /Products/Calculate
    [HttpGet]
    public async Task<IActionResult> Calculate()
    {
        var model = new CalculationViewModel();

        await FillTypeListsAsync(model);

        return View(model);
    }

    // POST: /Products/Calculate
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Calculate(CalculationViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await FillTypeListsAsync(model);

            return View(model);
        }

        model.Result = await _calculator.Calculate(
            model.ProductTypeId,
            model.MaterialTypeId,
            model.RawQuantity,
            model.Parameter1,
            model.Parameter2);

        if (model.Result < 0)
        {
            ModelState.AddModelError(
                string.Empty,
                "Расчёт невозможен. Проверьте, что выбраны существующие типы продукции и материала, " +
                "количество сырья — целое положительное число, а параметры продукции — положительные числа.");

            await FillTypeListsAsync(model);

            return View(model);
        }

        model.ProductTypeName = await _context.ProductTypes
            .Where(t => t.ProductTypeId == model.ProductTypeId)
            .Select(t => t.Name)
            .FirstOrDefaultAsync();

        model.MaterialTypeName = await _context.MaterialTypes
            .Where(t => t.MaterialTypeId == model.MaterialTypeId)
            .Select(t => t.Name)
            .FirstOrDefaultAsync();

        await FillTypeListsAsync(model);

        return View(model);
    }

    private async Task FillTypeListsAsync(CalculationViewModel model)
    {
        model.ProductTypes = await _context.ProductTypes
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .Select(t => new SelectListItem
            {
                Value = t.ProductTypeId.ToString(),
                Text = t.Name
            })
            .ToListAsync();

        model.MaterialTypes = await _context.MaterialTypes
            .AsNoTracking()
            .OrderBy(t => t.Name)
            .Select(t => new SelectListItem
            {
                Value = t.MaterialTypeId.ToString(),
                Text = t.Name
            })
            .ToListAsync();
    }
}
