using AromaDom.Data;
using AromaDom.Models;
using AromaDom.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace AromaDom.Controllers;


public class MaterialsController : Controller
{

    private const decimal MaxPrice = 9_999_999_999m;


    private const decimal MaxQuantity = 999_999_999m;

    private readonly AromaDomDevContext _context;

    public MaterialsController(AromaDomDevContext context)
    {
        _context = context;
    }


    public async Task<IActionResult> Index()
    {
        var materials = await _context.Materials
            .AsNoTracking()
            .OrderBy(m => m.MaterialType.Name)
            .ThenBy(m => m.Name)
            .Select(m => new MaterialViewModel
            {
                MaterialId = m.MaterialId,
                TypeName = m.MaterialType.Name,
                Name = m.Name,
                MinQuantity = m.MinQuantity,
                StockQuantity = m.StockQuantity,
                Price = m.Price,
                Unit = m.Unit,

                
                RequiredQuantity = m.MaterialProducts
                    .Sum(mp => (decimal?)mp.MaterialQuantity) ?? 0m
            })
            .ToListAsync();

        return View(materials);
    }

    // GET: /Materials/Create
    public async Task<IActionResult> Create()
    {
        var model = new MaterialEditViewModel();

        await FillMaterialTypesAsync(model);

        return View(model);
    }

    // POST: /Materials/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(MaterialEditViewModel model)
    {
        Normalize(model);

        await ValidateMaterialAsync(model, currentMaterialId: null);

        if (!ModelState.IsValid)
        {
            await FillMaterialTypesAsync(model);

            return View(model);
        }

        var material = new Material
        {
            MaterialTypeId = model.MaterialTypeId,
            Name = model.Name,
            Price = model.Price,
            Unit = model.Unit,
            PackageQuantity = model.PackageQuantity,
            StockQuantity = model.StockQuantity,
            MinQuantity = model.MinQuantity
        };

        _context.Materials.Add(material);

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            
            ModelState.AddModelError(
                nameof(model.Name),
                "Материал с таким наименованием уже существует. Укажите другое наименование.");

            await FillMaterialTypesAsync(model);

            return View(model);
        }

        TempData["SuccessMessage"] =
            $"Материал «{material.Name}» добавлен. Данные в списке материалов обновлены.";

        return RedirectToAction(nameof(Index));
    }

    // GET: /Materials/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var material = await _context.Materials
            .AsNoTracking()
            .FirstOrDefaultAsync(m => m.MaterialId == id);

        if (material is null)
        {
            TempData["ErrorMessage"] =
                $"Материал с идентификатором {id} не найден. Возможно, он был удалён. Выберите материал из списка.";

            return RedirectToAction(nameof(Index));
        }

        var model = new MaterialEditViewModel
        {
            Id = material.MaterialId,
            MaterialTypeId = material.MaterialTypeId,
            Name = material.Name,
            Price = material.Price,
            Unit = material.Unit,
            PackageQuantity = material.PackageQuantity,
            StockQuantity = material.StockQuantity,
            MinQuantity = material.MinQuantity
        };

        await FillMaterialTypesAsync(model);

        return View(model);
    }

    // POST: /Materials/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, MaterialEditViewModel model)
    {
        model.Id = id;

        Normalize(model);

        var material = await _context.Materials
            .FirstOrDefaultAsync(m => m.MaterialId == id);

        if (material is null)
        {
            TempData["ErrorMessage"] =
                $"Материал с идентификатором {id} не найден. Возможно, он был удалён. Выберите материал из списка.";

            return RedirectToAction(nameof(Index));
        }

        await ValidateMaterialAsync(model, currentMaterialId: id);

        if (!ModelState.IsValid)
        {
            await FillMaterialTypesAsync(model);

            return View(model);
        }

        material.MaterialTypeId = model.MaterialTypeId;
        material.Name = model.Name;
        material.Price = model.Price;
        material.Unit = model.Unit;
        material.PackageQuantity = model.PackageQuantity;
        material.StockQuantity = model.StockQuantity;
        material.MinQuantity = model.MinQuantity;

        try
        {
            await _context.SaveChangesAsync();
        }
        catch (DbUpdateException)
        {
            ModelState.AddModelError(
                nameof(model.Name),
                "Материал с таким наименованием уже существует. Укажите другое наименование.");

            await FillMaterialTypesAsync(model);

            return View(model);
        }

        TempData["SuccessMessage"] =
            $"Изменения материала «{material.Name}» сохранены. Данные в списке материалов обновлены.";

        return RedirectToAction(nameof(Index));
    }

    // GET: /Materials/Products/5 — продукция, в рецептуре которой используется материал.
    public async Task<IActionResult> Products(int id)
    {
        var model = await _context.Materials
            .AsNoTracking()
            .Where(m => m.MaterialId == id)
            .Select(m => new MaterialProductsViewModel
            {
                MaterialId = m.MaterialId,
                MaterialName = m.Name,
                Unit = m.Unit,
                Products = m.MaterialProducts
                    .OrderBy(mp => mp.Product.Name)
                    .Select(mp => new ProductUsageViewModel
                    {
                        ProductId = mp.ProductId,
                        ProductName = mp.Product.Name,
                        Article = mp.Product.Article,
                        RequiredQuantity = mp.MaterialQuantity
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (model is null)
        {
            TempData["ErrorMessage"] =
                $"Материал с идентификатором {id} не найден. Возможно, он был удалён. Выберите материал из списка.";

            return RedirectToAction(nameof(Index));
        }

        return View(model);
    }


    private static void Normalize(MaterialEditViewModel model)
    {
        model.Name = model.Name?.Trim() ?? string.Empty;
        model.Unit = model.Unit?.Trim() ?? string.Empty;
        model.Price = Math.Round(model.Price, 2, MidpointRounding.AwayFromZero);
        model.PackageQuantity = Math.Round(model.PackageQuantity, 3, MidpointRounding.AwayFromZero);
        model.StockQuantity = Math.Round(model.StockQuantity, 3, MidpointRounding.AwayFromZero);
        model.MinQuantity = Math.Round(model.MinQuantity, 3, MidpointRounding.AwayFromZero);

        if (model.Price > MaxPrice)
        {
            model.Price = MaxPrice;
        }

        if (model.PackageQuantity > MaxQuantity)
        {
            model.PackageQuantity = MaxQuantity;
        }

        if (model.StockQuantity > MaxQuantity)
        {
            model.StockQuantity = MaxQuantity;
        }

        if (model.MinQuantity > MaxQuantity)
        {
            model.MinQuantity = MaxQuantity;
        }
    }


    private async Task ValidateMaterialAsync(MaterialEditViewModel model, int? currentMaterialId)
    {
        bool typeExists = await _context.MaterialTypes
            .AnyAsync(t => t.MaterialTypeId == model.MaterialTypeId);

        if (!typeExists)
        {
            ModelState.AddModelError(
                nameof(model.MaterialTypeId),
                "Выбранный тип материала не найден. Обновите страницу и выберите тип из выпадающего списка.");
        }

        bool nameIsTaken = await _context.Materials
            .AnyAsync(m => m.Name == model.Name &&
                           (currentMaterialId == null || m.MaterialId != currentMaterialId));

        if (nameIsTaken)
        {
            ModelState.AddModelError(
                nameof(model.Name),
                "Материал с таким наименованием уже существует. Укажите другое наименование.");
        }
    }

    private async Task FillMaterialTypesAsync(MaterialEditViewModel model)
    {
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
