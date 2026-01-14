using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
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

    [ObservableProperty]
    private ObservableCollection<FoodEaten> eaten = new();

    [ObservableProperty]
    private bool isAddFoodDialogVisible;

    public decimal TotalCalories => Current?.TotalCalories() ?? 0;
    public decimal TotalProtein => Current?.TotalProtein() ?? 0;
    public decimal TotalFat => Current?.TotalFat() ?? 0;
    public decimal TotalCarbs => Current?.TotalCarbs() ?? 0;

    [ObservableProperty]
    private AddFoodDialogViewModel? addFoodDialogViewModel;

    private void RefreshTotals()
    {
        OnPropertyChanged(nameof(TotalCalories));
        OnPropertyChanged(nameof(TotalProtein));
        OnPropertyChanged(nameof(TotalFat));
        OnPropertyChanged(nameof(TotalCarbs));
    }

    private async Task NavigateToDay(int dayOffset)
    {
        using var db = new UserContext();
        var targetDate = Current!.Date.AddDays(dayOffset);
        var day = await db.Day.Include(x => x.FoodsEaten).ThenInclude(x => x.Preset).FirstOrDefaultAsync(x => x.Date == targetDate);
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
        RefreshTotals();
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
        Current = day;
        Eaten = new ObservableCollection<FoodEaten>(Current.FoodsEaten);
        // always update to latest weight/goal on daily but not on nav
        Current.Weight = db.Weight.OrderByDescending(x => x.Created).FirstOrDefault();
        Current.Goal = db.Goal.OrderByDescending(x => x.Created).FirstOrDefault();
        db.SaveChanges();
        RefreshTotals();
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
        RefreshTotals();
    }


    [RelayCommand]
    private async Task RemoveFood(FoodEaten food)
    {
        using var db = new UserContext();
        Current!.FoodsEaten.Remove(food);
        db.Day.Update(Current);
        await db.SaveChangesAsync();
        Eaten.Remove(food);
        RefreshTotals();
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
}
