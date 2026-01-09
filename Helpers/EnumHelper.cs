using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace DierentuinApp.Helpers;

// Helper class om de Nederlandse display naam van enums op te halen
public static class EnumHelper
{
    /// <summary>
    /// Haalt de [Display(Name = "...")] waarde op van een enum, of de enum naam als er geen display attribute is
    /// </summary>
    public static string GetDisplayName(this Enum enumValue)
    {
        var displayAttribute = enumValue.GetType()
            .GetMember(enumValue.ToString())
            .FirstOrDefault()?
            .GetCustomAttribute<DisplayAttribute>();

        return displayAttribute?.Name ?? enumValue.ToString();
    }
}

