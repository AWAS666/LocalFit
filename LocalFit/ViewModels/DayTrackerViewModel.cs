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
    private ObservableCollection<FoodEaten> eaten;

    public DayTrackerViewModel()
    {
        var today = DateOnly.FromDateTime(DateTime.UtcNow);
        using var db = new UserContext();
        Current = db.Day.Include(x => x.FoodsEaten).FirstOrDefault(x => x.Date == today);
        if (Current == null)
        {
            Current = new DayTracker
            {
                Date = today,
            };
            db.Day.Add(Current);
            Eaten = new ObservableCollection<FoodEaten>(Current.FoodsEaten);
        }
        // always update to latest weight/goal on daily but not on nav
        Current.Weight = db.Weight.OrderByDescending(x => x.Created).FirstOrDefault();
        Current.Goal = db.Goal.OrderByDescending(x => x.Created).FirstOrDefault();
        db.SaveChanges();
    }


    [RelayCommand]
    private async Task AddFood()
    {
        using var db = new UserContext();
        var foodEaten = new FoodEaten
        {
        };
        Current!.FoodsEaten.Add(foodEaten);
        db.Day.Update(Current);
        await db.SaveChangesAsync();
        Eaten.Add(foodEaten);
    }


    [RelayCommand]
    private async Task RemoveFood(FoodEaten food)
    {
        using var db = new UserContext();
        Current!.FoodsEaten.Remove(food);
        db.Day.Update(Current);
        await db.SaveChangesAsync();
        Eaten.Remove(food);
    }

    [RelayCommand]
    private async Task LastDay()
    {
        using var db = new UserContext();
        var previousDate = Current!.Date.AddDays(-1);
        var last = await db.Day.Include(x => x.FoodsEaten).FirstOrDefaultAsync(x => x.Date == previousDate);
        if (last == null) return;
        Current = last;
        Eaten = new ObservableCollection<FoodEaten>(Current.FoodsEaten);
    }

    [RelayCommand]
    private async Task NextDay()
    {
        using var db = new UserContext();
        var previousDate = Current!.Date.AddDays(1);
        var last = await db.Day.Include(x => x.FoodsEaten).FirstOrDefaultAsync(x => x.Date == previousDate);
        if (last == null) return;
        Current = last;
        Eaten = new ObservableCollection<FoodEaten>(Current.FoodsEaten);
    }
}
