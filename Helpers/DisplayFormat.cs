using System.Globalization;

namespace AromaDom.Helpers;


public static class DisplayFormat
{
    private static readonly CultureInfo RussianCulture = CultureInfo.GetCultureInfo("ru-RU");

    public static string Number(decimal value)
    {
        return value.ToString("N2", RussianCulture);
    }

    public static string Quantity(decimal value)
    {
        return value.ToString("N2", RussianCulture);
    }

    public static string Money(decimal value)
    {
        return $"{value.ToString("N2", RussianCulture)} ₽";
    }
}
