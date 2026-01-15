using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalFit.Lib;
using LocalFit.Lib.DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalFit.ViewModels;

public partial class DayTrackerViewModel : ObservableObject
{
    /// <summary>
    /// defaults to current, can navigate days -> add option to just pick date?
    /// </summary>
    [ObservableProperty]
    private DayTracker current;

    partial void OnCurrentChanged(DayTracker value)
    {
        GoalNutrition = CalculateGoalNutrition();
        Update();
    }

    [ObservableProperty]
    private ObservableCollection<FoodEaten> eaten = new();

    [ObservableProperty]
    private bool isAddFoodDialogVisible;

    public decimal TotalCalories => Current?.TotalCalories() ?? 0;
    public decimal TotalProtein => Current?.TotalProtein() ?? 0;
    public decimal TotalFat => Current?.TotalFat() ?? 0;
    public decimal TotalCarbs => Current?.TotalCarbs() ?? 0;

    [ObservableProperty]
    private NutritionResult goalNutrition;

    public string CaloriesDisplay => GetDisplay(TotalCalories, GoalNutrition.Calories, 0);
    public string ProteinDisplay => GetDisplay(TotalProtein, GoalNutrition.ProteinGrams, 1);
    public string FatDisplay => GetDisplay(TotalFat, GoalNutrition.FatGrams, 1);
    public string CarbsDisplay => GetDisplay(TotalCarbs, GoalNutrition.CarbsGrams, 1);

    public string CaloriesForegroundColor => GetForegroundColor(TotalCalories, GoalNutrition.Calories);
    public string ProteinForegroundColor => GetForegroundColor(TotalProtein, GoalNutrition.ProteinGrams);
    public string FatForegroundColor => GetForegroundColor(TotalFat, GoalNutrition.FatGrams);
    public string CarbsForegroundColor => GetForegroundColor(TotalCarbs, GoalNutrition.CarbsGrams);

    private static string GetDisplay(decimal current, decimal goal, int decimals)
    {
        if (goal <= 0) return $"{current:F0}";
        return decimals == 0
            ? $"{current:F0}/{goal:F0}"
            : $"{current:F0}/{goal:F0}";
    }

    private static string GetForegroundColor(decimal current, decimal goal)
    {
        // Show red text when exceeding goal
        if (goal > 0 && current > goal)
            return "#FFE53935";
        return "#FFFFFF";
    }

    [ObservableProperty]
    private AddFoodDialogViewModel? addFoodDialogViewModel;

    [ObservableProperty]
    private bool isEditEatenFoodDialogVisible;

    [ObservableProperty]
    private EditEatenFoodDialogViewModel? editEatenFoodDialogViewModel;


    private NutritionResult CalculateGoalNutrition()
    {
        if (Current?.Weight == null || Current?.Goal == null)
            return new NutritionResult();

        return BMRCalculator.CalculateDailyNutrition(
            Current.Weight.Weight,
            Current.Weight.Height,
            Current.Weight.Age,
            Current.Goal.Sex,
            Current.Goal.FitLevel,
            Current.Goal.Goal
        );
    }

    private async Task NavigateToDay(int dayOffset)
    {
        using var db = new UserContext();
        var targetDate = Current!.Date.AddDays(dayOffset);
        var day = await db.Day.Include(x => x.FoodsEaten)
            .ThenInclude(x => x.Preset)
            .Include(x => x.Goal)
            .Include(x => x.Weight)
            .FirstOrDefaultAsync(x => x.Date == targetDate);
        if (day == null)
        {
            day = new DayTracker
            {
                Date = targetDate,
                // set to current if this was missing
                Weight = db.Weight.OrderByDescending(x => x.Created).FirstOrDefault(),
                Goal = db.Goal.OrderByDescending(x => x.Created).FirstOrDefault()
            };
            db.Day.Add(day);
            await db.SaveChangesAsync();
        }
        Current = day;
        Eaten = new ObservableCollection<FoodEaten>(Current.FoodsEaten);
    }

    public DayTrackerViewModel()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        using var db = new UserContext();
        var day = db.Day.Include(x => x.FoodsEaten).ThenInclude(x => x.Preset).FirstOrDefault(x => x.Date == today);
        if (day == null)
        {
            day = new DayTracker
            {
                Date = today,
            };
            db.Day.Add(day);
        }
        // always update to latest weight/goal on daily but not on nav
        day.Weight = db.Weight.OrderByDescending(x => x.Created).FirstOrDefault();
        day.Goal = db.Goal.OrderByDescending(x => x.Created).FirstOrDefault();
        Current = day;
        Eaten = new ObservableCollection<FoodEaten>(Current.FoodsEaten);
        db.SaveChanges();
    }

    private void SetupAddFoodDialog(AddFoodDialogViewModel dialogViewModel)
    {
        dialogViewModel.OnFoodSelected += async (preset, amount) =>
        {
            await AddFoodToDay(preset, amount);
            IsAddFoodDialogVisible = false;
        };
        dialogViewModel.OnCancel += () =>
        {
            IsAddFoodDialogVisible = false;
        };
        IsAddFoodDialogVisible = true;
    }

    [RelayCommand]
    private void ShowAddFoodDialog()
    {
        AddFoodDialogViewModel = new AddFoodDialogViewModel();
        SetupAddFoodDialog(AddFoodDialogViewModel);
    }

    [RelayCommand]
    private void HideAddFoodDialog()
    {
        IsAddFoodDialogVisible = false;
        AddFoodDialogViewModel = null;
    }

    private async Task AddFoodToDay(FoodPresets preset, decimal amount)
    {
        using var db = new UserContext();
        db.Attach(Current!);
        var presetEntity = await db.FoodPresets.FindAsync(preset.Id);
        if (presetEntity == null) return;

        // Increment use count and save last amount
        presetEntity.UseCount++;
        presetEntity.LastAmount = amount;
        presetEntity.Updated = DateTime.UtcNow;
        db.FoodPresets.Update(presetEntity);

        var foodEaten = new FoodEaten
        {
            Preset = presetEntity,
            Amount = amount
        };
        Current!.FoodsEaten.Add(foodEaten);
        db.Day.Update(Current);
        await db.SaveChangesAsync();

        // Reload to get the entity with Preset loaded
        db.Entry(foodEaten).Reference(x => x.Preset).Load();
        Eaten.Add(foodEaten);
        Update();
    }

    private void Update()
    {
        OnPropertyChanged(nameof(TotalCalories));
        OnPropertyChanged(nameof(TotalProtein));
        OnPropertyChanged(nameof(TotalFat));
        OnPropertyChanged(nameof(TotalCarbs));
        OnPropertyChanged(nameof(CaloriesDisplay));
        OnPropertyChanged(nameof(ProteinDisplay));
        OnPropertyChanged(nameof(FatDisplay));
        OnPropertyChanged(nameof(CarbsDisplay));
        OnPropertyChanged(nameof(CaloriesForegroundColor));
        OnPropertyChanged(nameof(ProteinForegroundColor));
        OnPropertyChanged(nameof(FatForegroundColor));
        OnPropertyChanged(nameof(CarbsForegroundColor));
    }

    [RelayCommand]
    private async Task RemoveFood(FoodEaten food)
    {
        using var db = new UserContext();
        db.Attach(Current!);
        Current!.FoodsEaten.Remove(food);
        db.Day.Update(Current);
        await db.SaveChangesAsync();
        Eaten.Remove(food);
        Update();
    }

    [RelayCommand]
    private async Task LastDay()
    {
        await NavigateToDay(-1);
    }

    [RelayCommand]
    private async Task NextDay()
    {
        await NavigateToDay(1);
    }

    private void SetupEditEatenFoodDialog(EditEatenFoodDialogViewModel dialogViewModel)
    {
        dialogViewModel.OnSaved += (foodEaten) =>
        {
            // Update the item in the collection
            var index = Eaten.IndexOf(foodEaten);
            if (index >= 0)
            {
                Eaten[index] = foodEaten;
            }
            IsEditEatenFoodDialogVisible = false;
            EditEatenFoodDialogViewModel = null;
            OnPropertyChanged(nameof(TotalCalories));
            OnPropertyChanged(nameof(TotalProtein));
            OnPropertyChanged(nameof(TotalFat));
            OnPropertyChanged(nameof(TotalCarbs));
            OnPropertyChanged(nameof(CaloriesDisplay));
            OnPropertyChanged(nameof(ProteinDisplay));
            OnPropertyChanged(nameof(FatDisplay));
            OnPropertyChanged(nameof(CarbsDisplay));
            OnPropertyChanged(nameof(CaloriesForegroundColor));
            OnPropertyChanged(nameof(ProteinForegroundColor));
            OnPropertyChanged(nameof(FatForegroundColor));
            OnPropertyChanged(nameof(CarbsForegroundColor));
        };
        dialogViewModel.OnCancel += () =>
        {
            IsEditEatenFoodDialogVisible = false;
            EditEatenFoodDialogViewModel = null;
        };
        IsEditEatenFoodDialogVisible = true;
    }

    [RelayCommand]
    private void ShowEditEatenFoodDialog(FoodEaten foodEaten)
    {
        EditEatenFoodDialogViewModel = new EditEatenFoodDialogViewModel(foodEaten);
        SetupEditEatenFoodDialog(EditEatenFoodDialogViewModel);
    }

    [RelayCommand]
    private void HideEditEatenFoodDialog()
    {
        IsEditEatenFoodDialogVisible = false;
        EditEatenFoodDialogViewModel = null;
    }
}
