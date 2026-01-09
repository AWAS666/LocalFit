using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalFit.Lib.DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LocalFit.ViewModels;

public partial class FoodManagerViewModel : ObservableObject
{
    [ObservableProperty]
    private int page = 0;

    private int size = 20;

    [ObservableProperty]
    private List<FoodPresets> foods = new();

    public FoodManagerViewModel()
    {
        LoadFoods();
    }


    partial void OnPageChanged(int value)
    {
        LoadFoods();
    }


    private async Task LoadFoods()
    {
        using var db = new UserContext();
        Foods = await db.FoodPresets.Skip(Page * size).Take(size).ToListAsync();
    }

    [RelayCommand]
    private async Task Add()
    {
        using var db = new UserContext();
        db.FoodPresets.Add(new FoodPresets
        {
            Name = "New Food",
            Calories = 0,
            Fat = 0,
            Protein = 0,
            Carbs = 0,
            Sugar = 0,
        });
        await db.SaveChangesAsync();
        await LoadFoods();
    }
}
