using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AromaDom.ViewModels;


public class MaterialEditViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Выберите тип материала из выпадающего списка.")]
    [Range(1, int.MaxValue, ErrorMessage = "Выберите тип материала из выпадающего списка.")]
    [Display(Name = "Тип материала")]
    public int MaterialTypeId { get; set; }

    [Required(ErrorMessage = "Введите наименование материала.")]
    [StringLength(200, ErrorMessage = "Наименование не должно быть длиннее 200 символов.")]
    [Display(Name = "Наименование")]
    public string Name { get; set; } = string.Empty;

    [Range(typeof(decimal), "0", "9999999999", ErrorMessage = "Цена не может быть отрицательной.")]
    [Display(Name = "Цена за единицу")]
    public decimal Price { get; set; }

    [Required(ErrorMessage = "Укажите единицу измерения, например кг или шт.")]
    [StringLength(30, ErrorMessage = "Единица измерения не должна быть длиннее 30 символов.")]
    [Display(Name = "Единица измерения")]
    public string Unit { get; set; } = string.Empty;

    [Range(typeof(decimal), "0.001", "999999999", ErrorMessage = "Количество в упаковке должно быть больше нуля.")]
    [Display(Name = "Количество в упаковке")]
    public decimal PackageQuantity { get; set; }

    [Range(typeof(decimal), "0", "999999999", ErrorMessage = "Количество на складе не может быть отрицательным.")]
    [Display(Name = "Количество на складе")]
    public decimal StockQuantity { get; set; }

    [Range(typeof(decimal), "0", "999999999", ErrorMessage = "Минимальное количество не может быть отрицательным.")]
    [Display(Name = "Минимальное количество")]
    public decimal MinQuantity { get; set; }


    public List<SelectListItem> MaterialTypes { get; set; } = new();
}
