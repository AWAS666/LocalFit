using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalFit.Extensions;
using LocalFit.Helpers;
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

    private readonly List<EnumDisplayItem<Sex>> _sexes = [.. EnumHelper.GetDisplayItems<Sex>()];
    private readonly List<EnumDisplayItem<FitnessLevel>> _fitnessLevels = [.. EnumHelper.GetDisplayItems<FitnessLevel>()];
    private readonly List<EnumDisplayItem<CalorieGoal>> _goals = [.. EnumHelper.GetDisplayItems<CalorieGoal>()];
    private readonly List<EnumDisplayItem<DietType>> _dietTypes = [.. EnumHelper.GetDisplayItems<DietType>()];

    public IEnumerable<EnumDisplayItem<Sex>> Sexes => _sexes;
    public IEnumerable<EnumDisplayItem<FitnessLevel>> FitnessLevels => _fitnessLevels;
    public IEnumerable<EnumDisplayItem<CalorieGoal>> Goals => _goals;
    public IEnumerable<EnumDisplayItem<DietType>> DietTypes => _dietTypes;

    private EnumDisplayItem<Sex>? _selectedSexItem;
    private EnumDisplayItem<FitnessLevel>? _selectedFitnessItem;
    private EnumDisplayItem<CalorieGoal>? _selectedGoalItem;
    private EnumDisplayItem<DietType>? _selectedDietTypeItem;

    public EnumDisplayItem<Sex>? SelectedSexItem
    {
        get => _selectedSexItem;
        set
        {
            if (SetProperty(ref _selectedSexItem, value) && value != null)
            {
                Sex = value.Value;
            }
        }
    }

    public EnumDisplayItem<FitnessLevel>? SelectedFitnessItem
    {
        get => _selectedFitnessItem;
        set
        {
            if (SetProperty(ref _selectedFitnessItem, value) && value != null)
            {
                Fitness = value.Value;
            }
        }
    }

    public EnumDisplayItem<CalorieGoal>? SelectedGoalItem
    {
        get => _selectedGoalItem;
        set
        {
            if (SetProperty(ref _selectedGoalItem, value) && value != null)
            {
                Goal = value.Value;
            }
        }
    }

    public EnumDisplayItem<DietType>? SelectedDietTypeItem
    {
        get => _selectedDietTypeItem;
        set
        {
            if (SetProperty(ref _selectedDietTypeItem, value) && value != null)
            {
                DietType = value.Value;
            }
        }
    }

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

        UpdateSelectedItems();
    }

    private void UpdateSelectedItems()
    {
        SelectedSexItem = Sex.HasValue ? _sexes.FirstOrDefault(s => s.Value == Sex.Value) : null;
        SelectedFitnessItem = Fitness.HasValue ? _fitnessLevels.FirstOrDefault(f => f.Value == Fitness.Value) : null;
        SelectedGoalItem = Goal.HasValue ? _goals.FirstOrDefault(g => g.Value == Goal.Value) : null;
        SelectedDietTypeItem = DietType.HasValue ? _dietTypes.FirstOrDefault(d => d.Value == DietType.Value) : null;
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
        SelectedSexItem = newValue.HasValue ? _sexes.FirstOrDefault(s => s.Value == newValue.Value) : null;
    }

    partial void OnFitnessChanged(FitnessLevel? oldValue, FitnessLevel? newValue)
    {
        Recalculate();
        SelectedFitnessItem = newValue.HasValue ? _fitnessLevels.FirstOrDefault(f => f.Value == newValue.Value) : null;
    }

    partial void OnGoalChanged(CalorieGoal? oldValue, CalorieGoal? newValue)
    {
        Recalculate();
        SelectedGoalItem = newValue.HasValue ? _goals.FirstOrDefault(g => g.Value == newValue.Value) : null;
    }

    partial void OnDietTypeChanged(DietType? oldValue, DietType? newValue)
    {
        Recalculate();
        SelectedDietTypeItem = newValue.HasValue ? _dietTypes.FirstOrDefault(d => d.Value == newValue.Value) : null;
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
