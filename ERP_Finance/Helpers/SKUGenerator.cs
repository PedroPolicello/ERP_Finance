using ERP_Finance.Models;
using ERP_Finance.Types;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;

namespace ERP_Finance.Helpers;

public static class SKUGenerator
{
    public static string GenerateSKU()
    {
        var sku = new StringBuilder();

        for (int i = 0; i < 9; i++)
        {
            sku.Append(RandomNumberGenerator.GetInt32(10));

            if (i == 2 || i == 5)
                sku.Append('-');
        }

        return sku.ToString();
    }
}