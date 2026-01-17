using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalFit.Lib;
using LocalFit.Lib.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace LocalFit.ViewModels;

public partial class BMRViewModel : ObservableObject
{
    private readonly MainViewModel mainViewModel;

    [ObservableProperty]
    private decimal? weight = 60;

    [ObservableProperty]
    private decimal? height;

    [ObservableProperty]
    private int? age;

    [ObservableProperty]
    private Sex? sex;

    [ObservableProperty]
    private FitnessLevel? fitness;

    [ObservableProperty]
    private CalorieGoal? goal;

    [ObservableProperty]
    private DietType? dietType;

    public Sex[] Sexes => Enum.GetValues<Sex>();
    public FitnessLevel[] FitnessLevels => Enum.GetValues<FitnessLevel>();
    public CalorieGoal[] Goals => Enum.GetValues<CalorieGoal>();
    public DietType[] DietTypes => Enum.GetValues<DietType>();

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

    public BMRViewModel(MainViewModel mainViewModel)
    {
        this.mainViewModel = mainViewModel;
        LoadUserDefaults();
        Recalculate();
    }

    private void LoadUserDefaults()
    {
        using var db = new UserContext();
        var currentWeight = db.GetLatestWeight();
        var currentGoal = db.GetLatestGoal();

        if (currentWeight != null)
        {
            Weight = currentWeight.Weight;
            Height = currentWeight.Height;
            Age = currentWeight.Age;
        }

        if (currentGoal != null)
        {
            Sex = currentGoal.Sex;
            Fitness = currentGoal.FitLevel;
            Goal = currentGoal.Goal;
            DietType = currentGoal.DietType;
        }
    }

    [RelayCommand]
    private void NavigateBack()
    {
        mainViewModel.NavigateToSettings();
    }

    partial void OnWeightChanged(decimal? oldValue, decimal? newValue)
    {
        Recalculate();
    }

    partial void OnHeightChanged(decimal? oldValue, decimal? newValue)
    {
        Recalculate();
    }

    partial void OnAgeChanged(int? oldValue, int? newValue)
    {
        Recalculate();
    }

    partial void OnSexChanged(Sex? oldValue, Sex? newValue)
    {
        Recalculate();
    }

    partial void OnFitnessChanged(FitnessLevel? oldValue, FitnessLevel? newValue)
    {
        Recalculate();
    }

    partial void OnGoalChanged(CalorieGoal? oldValue, CalorieGoal? newValue)
    {
        Recalculate();
    }

    partial void OnDietTypeChanged(DietType? oldValue, DietType? newValue)
    {
        Recalculate();
    }

    private void Recalculate()
    {
        if (Weight == null || Height == null || Age == null || Sex == null || Fitness == null || Goal == null || DietType == null)
        {
            Calories = null;
            Protein = null;
            Fat = null;
            Carbs = null;
            Bmi = null;
            return;
        }

        var nut = BMRCalculator.CalculateDailyNutrition(Weight.Value, Height.Value, Age.Value, Sex.Value, Fitness.Value, Goal.Value, DietType.Value);
        Calories = nut.Calories;
        Protein = nut.ProteinGrams;
        Fat = nut.FatGrams;
        Carbs = nut.CarbsGrams;
        Bmi = BMRCalculator.BMI(Weight.Value, Height.Value);
    }
}
