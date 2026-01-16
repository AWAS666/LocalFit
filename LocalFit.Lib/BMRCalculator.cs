using System;
using System.Collections.Generic;
using System.Runtime.Intrinsics.X86;
using System.Text;

namespace LocalFit.Lib;

/// <summary>
/// todo: need to read up on actual nutrition calculations and improve this -> there should be papers on this but will do as a start
/// https://www.calculator.net/macro-calculator.html -> seems to have similarish results
/// </summary>
public static class BMRCalculator
{
    private static decimal CalculateBmr(decimal weightKg, decimal heightCm, int age, Sex sex)
    {
        return sex == Sex.Male
            ? (10 * weightKg) + (6.25m * heightCm) - (5 * age) + 5
            : (10 * weightKg) + (6.25m * heightCm) - (5 * age) - 161;
    }
    private static decimal GetProteinPerKg(FitnessLevel level)
    {
        return level switch
        {
            FitnessLevel.Sedentary => 1.6m,
            FitnessLevel.LightlyActive => 1.8m,
            FitnessLevel.ModeratelyActive => 2.0m,
            FitnessLevel.VeryActive => 2.2m,
            FitnessLevel.Athlete => 2.4m,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    private static decimal GetActivityMultiplier(FitnessLevel level)
    {
        return level switch
        {
            FitnessLevel.Sedentary => 1.2m,
            FitnessLevel.LightlyActive => 1.375m,
            FitnessLevel.ModeratelyActive => 1.55m,
            FitnessLevel.VeryActive => 1.725m,
            FitnessLevel.Athlete => 1.9m,
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    /// <summary>
    /// this might have to be adjusted based on body weight
    /// </summary>
    /// <param name="goal"></param>
    /// <returns></returns>
    private static decimal GetGoalCaloriesAdjustment(CalorieGoal goal)
    {
        return goal switch
        {
            CalorieGoal.AggressiveCut => -500,
            CalorieGoal.Cut => -250,
            CalorieGoal.Maintenance => 0,
            CalorieGoal.Bulk => 150,
            CalorieGoal.AggressiveBulk => 300,
            _ => throw new NotImplementedException(),
        };
    }


    public static NutritionResult CalculateDailyNutrition(decimal weightKg, decimal heightCm, int age, Sex sex, FitnessLevel fitnessLevel, CalorieGoal goal, DietType dietType = DietType.Balanced)
    {
        var bmr = CalculateBmr(weightKg, heightCm, age, sex);
        var tdee = bmr * GetActivityMultiplier(fitnessLevel);

        // Apply goal
        var targetCalories = tdee + GetGoalCaloriesAdjustment(goal);

        // Base protein calculation
        var baseProteinGrams = weightKg * GetProteinPerKg(fitnessLevel);
        var baseProteinCalories = baseProteinGrams * 4;

        // Adjust macros based on diet type
        decimal proteinCalories, fatCalories, carbsCalories;
        
        switch (dietType)
        {
            case DietType.Balanced:
                // Balanced: 25% fat, protein based on fitness level, rest carbs
                fatCalories = targetCalories * 0.25m;
                proteinCalories = baseProteinCalories;
                carbsCalories = targetCalories - proteinCalories - fatCalories;
                break;
                
            case DietType.LowFat:
                // LowFat: 15% fat, maintain protein, increase carbs
                fatCalories = targetCalories * 0.15m;
                proteinCalories = baseProteinCalories;
                carbsCalories = targetCalories - proteinCalories - fatCalories;
                break;
                
            case DietType.LowCarb:
                // LowCarb: 10% carbs, increase protein and fat
                carbsCalories = targetCalories * 0.10m;
                // Increase protein by 20% for low carb
                proteinCalories = baseProteinCalories * 1.2m;
                fatCalories = targetCalories - proteinCalories - carbsCalories;
                break;
                
            case DietType.HighProtein:
                // HighProtein: increase protein by 30%, reduce fat slightly, adjust carbs
                proteinCalories = baseProteinCalories * 1.3m;
                fatCalories = targetCalories * 0.20m;
                carbsCalories = targetCalories - proteinCalories - fatCalories;
                break;
                
            default:
                // Default to balanced
                fatCalories = targetCalories * 0.25m;
                proteinCalories = baseProteinCalories;
                carbsCalories = targetCalories - proteinCalories - fatCalories;
                break;
        }

        // Convert to grams
        var proteinGrams = proteinCalories / 4;
        var fatGrams = fatCalories / 9;
        var carbsGrams = carbsCalories / 4;

        return new NutritionResult
        {
            Calories = Math.Round(targetCalories),
            ProteinGrams = Math.Round(proteinGrams),
            FatGrams = Math.Round(fatGrams),
            CarbsGrams = Math.Round(carbsGrams)
        };
    }

    public static decimal BMI(decimal weightKg, decimal heightCm)
    {
        var heightM = heightCm / 100;
        return weightKg / (heightM * heightM);
    }
}

public class NutritionResult
{
    /// <summary>
    /// for db usage
    /// </summary>
    public int Id { get; set; }
    public decimal Calories { get; set; }
    public decimal ProteinGrams { get; set; }
    public decimal FatGrams { get; set; }
    public decimal CarbsGrams { get; set; }
}


public enum Sex
{
    Male,
    Female
}

public enum FitnessLevel
{
    Sedentary,
    LightlyActive,
    ModeratelyActive,
    VeryActive,
    Athlete
}

public static class FitnessLevelExtensions
{
    public static string GetDescription(this FitnessLevel level)
    {
        return level switch
        {
            FitnessLevel.Sedentary => "little or no exercise", 
            FitnessLevel.LightlyActive => "exercise 1-3 times a week",
            FitnessLevel.ModeratelyActive => "exercise 4-5 times a week",
            FitnessLevel.VeryActive => "daily exercise or intense exercise 3-4 times a week", 
            FitnessLevel.Athlete => "intense exercise 6-7 times a week",
            _ => ""
        };
    }
}

public enum CalorieGoal
{
    AggressiveCut,
    Cut,
    Maintenance,
    Bulk,
    AggressiveBulk
}

public enum DietType
{
    Balanced,
    LowFat,
    LowCarb,
    HighProtein
}