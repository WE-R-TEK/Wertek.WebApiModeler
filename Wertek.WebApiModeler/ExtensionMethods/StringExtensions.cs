using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Wertek.WebApiModeler.Helpers;
using Wertek.WebApiModeler.Models;

namespace Wertek.WebApiModeler.ExtensionMethods;

public static class StringExtensions
{
    public static string Capitalize(
        this string value)
    {
        if(string.IsNullOrEmpty(value))
            return value;

        if (value.Length == 1)
            return value.ToUpper();

        return value.Substring(0, 1).ToUpper() + value.Substring(1);
    }
}
