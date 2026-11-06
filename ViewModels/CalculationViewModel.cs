using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AromaDom.ViewModels;


public class CalculationViewModel
{
    [Range(1, int.MaxValue, ErrorMessage = "Выберите тип продукции.")]
    [Display(Name = "Тип продукции")]
    public int ProductTypeId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Выберите тип материала.")]
    [Display(Name = "Тип материала (сырья)")]
    public int MaterialTypeId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Количество сырья должно быть целым положительным числом.")]
    [Display(Name = "Количество сырья")]
    public int RawQuantity { get; set; } = 1;

    [Range(typeof(double), "0.000001", "1000000000", ErrorMessage = "Первый параметр продукции должен быть положительным числом.")]
    [Display(Name = "Параметр продукции 1")]
    public double Parameter1 { get; set; } = 1;

    [Range(typeof(double), "0.000001", "1000000000", ErrorMessage = "Второй параметр продукции должен быть положительным числом.")]
    [Display(Name = "Параметр продукции 2")]
    public double Parameter2 { get; set; } = 1;


    public int? Result { get; set; }

    public string? ProductTypeName { get; set; }

    public string? MaterialTypeName { get; set; }

    public List<SelectListItem> ProductTypes { get; set; } = new();

    public List<SelectListItem> MaterialTypes { get; set; } = new();
}
