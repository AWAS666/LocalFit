using LocalFit.Lib;
using LocalFit.Lang;

namespace LocalFit.Extensions;

public static class FitnessLevelExtensions
{
    public static string GetDescription(this FitnessLevel level)
    {
        var resourceKey = level switch
        {
            FitnessLevel.Sedentary => "FitnessLevel_Sedentary",
            FitnessLevel.LightlyActive => "FitnessLevel_LightlyActive",
            FitnessLevel.ModeratelyActive => "FitnessLevel_ModeratelyActive",
            FitnessLevel.VeryActive => "FitnessLevel_VeryActive",
            FitnessLevel.Athlete => "FitnessLevel_Athlete",
            _ => null
        };
        
        if (resourceKey == null) return "";
        
        return Resources.ResourceManager.GetString(resourceKey, Resources.Culture) ?? "";
    }
}
