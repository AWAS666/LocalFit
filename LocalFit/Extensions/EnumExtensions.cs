using LocalFit.Lib;
using LocalFit.Lang;

namespace LocalFit.Extensions;

public static class SexExtensions
{
    public static string GetDisplayName(this Sex sex)
    {
        var resourceKey = sex switch
        {
            Sex.Male => "Sex_Male",
            Sex.Female => "Sex_Female",
            _ => null
        };
        
        if (resourceKey == null) return sex.ToString();
        
        return Resources.ResourceManager.GetString(resourceKey, Resources.Culture) ?? sex.ToString();
    }
}

public static class FitnessLevelDisplayExtensions
{
    public static string GetDisplayName(this FitnessLevel level)
    {
        var resourceKey = level switch
        {
            FitnessLevel.Sedentary => "FitnessLevel_Sedentary_Display",
            FitnessLevel.LightlyActive => "FitnessLevel_LightlyActive_Display",
            FitnessLevel.ModeratelyActive => "FitnessLevel_ModeratelyActive_Display",
            FitnessLevel.VeryActive => "FitnessLevel_VeryActive_Display",
            FitnessLevel.Athlete => "FitnessLevel_Athlete_Display",
            _ => null
        };
        
        if (resourceKey == null) return level.ToString();
        
        return Resources.ResourceManager.GetString(resourceKey, Resources.Culture) ?? level.ToString();
    }
}

public static class CalorieGoalExtensions
{
    public static string GetDisplayName(this CalorieGoal goal)
    {
        var resourceKey = goal switch
        {
            CalorieGoal.AggressiveCut => "CalorieGoal_AggressiveCut",
            CalorieGoal.Cut => "CalorieGoal_Cut",
            CalorieGoal.Maintenance => "CalorieGoal_Maintenance",
            CalorieGoal.Bulk => "CalorieGoal_Bulk",
            CalorieGoal.AggressiveBulk => "CalorieGoal_AggressiveBulk",
            _ => null
        };
        
        if (resourceKey == null) return goal.ToString();
        
        return Resources.ResourceManager.GetString(resourceKey, Resources.Culture) ?? goal.ToString();
    }
}

public static class DietTypeExtensions
{
    public static string GetDisplayName(this DietType dietType)
    {
        var resourceKey = dietType switch
        {
            DietType.Balanced => "DietType_Balanced",
            DietType.LowFat => "DietType_LowFat",
            DietType.LowCarb => "DietType_LowCarb",
            DietType.HighProtein => "DietType_HighProtein",
            _ => null
        };
        
        if (resourceKey == null) return dietType.ToString();
        
        return Resources.ResourceManager.GetString(resourceKey, Resources.Culture) ?? dietType.ToString();
    }
}
