using System.Globalization;
using ERP_Finance.Models;
using ERP_Finance.Types;

namespace ERP_Finance.Helpers;

public static class SKUGenerator
{
    public static string GenerateSKU(string name, ProductDetails details)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);

        if (details is null)
            throw new ArgumentNullException(nameof(details), "Product details cannot be null.");
        

        var productCode = GetPrefix(name);
        var brandCode = GetPrefix(details.BrandName);
        var uniqueCode = Guid.NewGuid().ToString("N").Substring(0, 6).ToUpperInvariant();

        var (formattedMeasure, measureCode) = FormatUnit(details.WeightOrVolume, details.MeasureType);

        return $"{productCode}-{brandCode}-{formattedMeasure}{measureCode}-{uniqueCode}";
    }

    private static string GetPrefix(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "GEN";

        var normalizedValue = Normalize(value);

        if (normalizedValue.Length <= 3)
            return normalizedValue;

        return normalizedValue[..3];
    }

    private static string Normalize(string value)
    {
        return new string(value
                    .Trim()
                    .ToUpperInvariant()
                    .Where(char.IsLetterOrDigit)
                    .ToArray());
    }

    private static (string FormattedValue, string MeasureCode) FormatUnit(decimal value, MeasureType measureType)
    {
        switch (measureType)
        {
            case MeasureType.Unit:
                // Produtos unitários:
                // 1 coxinha, 1 esfiha, 1 salgadinho
                return ("1", "UN");

            case MeasureType.Gram:
                ValidateMeasure(value);
                return (FormatValue(value), "G");

            case MeasureType.Kilogram:
                ValidateMeasure(value);

                // 1 kg = 1000 g
                var grams = value * 1000;

                return (FormatValue(grams), "G");

            case MeasureType.Milliliter:
                ValidateMeasure(value);
                return (FormatValue(value), "ML");

            case MeasureType.Liter:
                ValidateMeasure(value);

                // 1 litro = 1000 ml
                var milliliters = value * 1000;

                return (FormatValue(milliliters), "ML");

            default:
                throw new ArgumentOutOfRangeException(nameof(measureType), measureType, "Invalid measure type.");
        }
    }

    private static string FormatValue(decimal value)
    {
        return value.ToString("0.###", CultureInfo.InvariantCulture).Replace(".", "P");
    }

    private static void ValidateMeasure(decimal value)
    {
        if (value <= 0)
            throw new ArgumentOutOfRangeException(nameof(value), value, "The measure must be greater than zero.");
    }
}