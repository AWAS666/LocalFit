using LocalFit.Extensions;
using LocalFit.Lib;
using System;
using System.Collections.Generic;
using System.Linq;

namespace LocalFit.Helpers;

public static class EnumHelper
{
    public static IEnumerable<EnumDisplayItem<T>> GetDisplayItems<T>() where T : struct, Enum
    {
        return Enum.GetValues<T>().Select(e => new EnumDisplayItem<T>(e, GetDisplayName(e)));
    }

    private static string GetDisplayName<T>(T enumValue) where T : Enum
    {
        return enumValue switch
        {
            Sex sex => sex.GetDisplayName(),
            FitnessLevel level => level.GetDisplayName(),
            CalorieGoal goal => goal.GetDisplayName(),
            DietType dietType => dietType.GetDisplayName(),
            _ => enumValue.ToString()
        };
    }
}
