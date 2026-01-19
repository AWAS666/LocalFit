using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalFit.Extensions;
using LocalFit.Lib;
using LocalFit.Lib.DataBase;
using System;
using System.Linq;
using System.Reflection;

namespace LocalFit.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly MainViewModel mainViewModel;

    [ObservableProperty]
    private FitnessGoal? currentGoal;

    public string? GoalDisplayName => CurrentGoal?.Goal.GetDisplayName();
    public string? FitnessLevelDisplayName => CurrentGoal?.FitLevel.GetDisplayName();
    public string? SexDisplayName => CurrentGoal?.Sex.GetDisplayName();
    public string? DietTypeDisplayName => CurrentGoal?.DietType.GetDisplayName();

    [ObservableProperty]
    private WeightTracker? currentWeight;

    [ObservableProperty]
    private bool isGoalSetupDialogVisible;

    [ObservableProperty]
    private GoalSetupDialogViewModel? goalSetupDialogViewModel;

    [ObservableProperty]
    private decimal calculatedCalories;

    [ObservableProperty]
    private decimal calculatedProtein;

    [ObservableProperty]
    private decimal calculatedFat;

    [ObservableProperty]
    private decimal calculatedCarbs;

    [ObservableProperty]
    private string version = "1.0.0";

    public SettingsViewModel(MainViewModel mainViewModel)
    {
        this.mainViewModel = mainViewModel;
        LoadCurrentSettings();
        LoadVersion();
    }

    private void LoadVersion()
    {
        var assemblyVersion = Assembly.GetExecutingAssembly().GetName().Version;
        if (assemblyVersion != null)
        {
            Version = $"Version {assemblyVersion.Major}.{assemblyVersion.Minor}.{assemblyVersion.Build}";
        }
    }

    private void LoadCurrentSettings()
    {
        using var db = new UserContext();
        CurrentGoal = db.GetLatestGoal();
        CurrentWeight = db.GetLatestWeight();
        CalculateNutrition();
        OnPropertyChanged(nameof(GoalDisplayName));
        OnPropertyChanged(nameof(FitnessLevelDisplayName));
        OnPropertyChanged(nameof(SexDisplayName));
        OnPropertyChanged(nameof(DietTypeDisplayName));
    }

    private void CalculateNutrition()
    {
        if (CurrentWeight == null || CurrentGoal == null) return;

        var nutrition = BMRCalculator.CalculateDailyNutrition(
            CurrentWeight.Weight,
            CurrentWeight.Height,
            CurrentWeight.Age,
            CurrentGoal.Sex,
            CurrentGoal.FitLevel,
            CurrentGoal.Goal,
            CurrentGoal.DietType
        );

        CalculatedCalories = nutrition.Calories;
        CalculatedProtein = nutrition.ProteinGrams;
        CalculatedFat = nutrition.FatGrams;
        CalculatedCarbs = nutrition.CarbsGrams;
    }

    private void SetupGoalDialog(GoalSetupDialogViewModel dialogViewModel)
    {
        dialogViewModel.OnSaved += (goal) =>
        {
            CurrentGoal = goal;
            CalculateNutrition();
            OnPropertyChanged(nameof(GoalDisplayName));
            OnPropertyChanged(nameof(FitnessLevelDisplayName));
            OnPropertyChanged(nameof(SexDisplayName));
            OnPropertyChanged(nameof(DietTypeDisplayName));
            IsGoalSetupDialogVisible = false;
            GoalSetupDialogViewModel = null;
        };
        dialogViewModel.OnCancel += () =>
        {
            IsGoalSetupDialogVisible = false;
            GoalSetupDialogViewModel = null;
        };
        IsGoalSetupDialogVisible = true;
    }

    [RelayCommand]
    private void ShowGoalSetupDialog()
    {
        GoalSetupDialogViewModel = new GoalSetupDialogViewModel();
        SetupGoalDialog(GoalSetupDialogViewModel);
    }

    [RelayCommand]
    private void HideGoalSetupDialog()
    {
        IsGoalSetupDialogVisible = false;
        GoalSetupDialogViewModel = null;
    }

    [RelayCommand]
    private void ShowNutritionCalculator()
    {
        mainViewModel.ChangeView(new BMRViewModel(mainViewModel));
    }
}
