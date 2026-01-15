using CommunityToolkit.Mvvm.ComponentModel;
using LocalFit.Lib;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace LocalFit.ViewModels;

public partial class BMRViewModel : ObservableObject
{
    [ObservableProperty]
    private decimal? weight = 60;

    [ObservableProperty]
    private decimal? height = 180;

    [ObservableProperty]
    private int age = 20;

    [ObservableProperty]
    private Sex sex = Sex.Male;

    [ObservableProperty]
    private FitnessLevel fitness = FitnessLevel.Sedentary;

    [ObservableProperty]
    private CalorieGoal goal = CalorieGoal.Maintenance;

    public Sex[] Sexes => Enum.GetValues<Sex>();
    public FitnessLevel[] FitnessLevels => Enum.GetValues<FitnessLevel>();
    public CalorieGoal[] Goals => Enum.GetValues<CalorieGoal>();

    [ObservableProperty]
    private decimal? calories;

    [ObservableProperty]
    private decimal? fat;

    [ObservableProperty]
    private decimal? protein;

    [ObservableProperty]
    private decimal? carbs;

    [ObservableProperty]
    private decimal? bmi;

    public BMRViewModel()
    {
        Recalculate();
    }

    partial void OnWeightChanged(decimal? oldValue, decimal? newValue)
    {
        Recalculate();
    }

    partial void OnHeightChanged(decimal? oldValue, decimal? newValue)
    {
        Recalculate();
    }

    partial void OnAgeChanged(int oldValue, int newValue)
    {
        Recalculate();
    }

    partial void OnSexChanged(Sex oldValue, Sex newValue)
    {
        Recalculate();
    }

    partial void OnFitnessChanged(FitnessLevel oldValue, FitnessLevel newValue)
    {
        Recalculate();
    }

    partial void OnGoalChanged(CalorieGoal oldValue, CalorieGoal newValue)
    {
        Recalculate();
    }

    private void Recalculate()
    {
        if (Weight == null || Height == null)
        {
            Calories = null;
            Protein = null;
            Fat = null;
            Carbs = null;
            Bmi = null;
            return;
        }

        var nut = BMRCalculator.CalculateDailyNutrition(Weight.Value, Height.Value, Age, Sex, Fitness, Goal);
        Calories = nut.Calories;
        Protein = nut.ProteinGrams;
        Fat = nut.FatGrams;
        Carbs = nut.CarbsGrams;
        Bmi = BMRCalculator.BMI(Weight.Value, Height.Value);
    }
}
