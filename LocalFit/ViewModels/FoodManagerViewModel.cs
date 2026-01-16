using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalFit.Lib.DataBase;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LocalFit.ViewModels;

public partial class FoodManagerViewModel : ObservableObject
{
    [ObservableProperty]
    private int page = 0;

    private int size = 10;

    [ObservableProperty]
    private ObservableCollection<FoodPresets> foods = new();

    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private bool isEditFoodDialogVisible;

    [ObservableProperty]
    private EditFoodDialogViewModel? editFoodDialogViewModel;

    private List<FoodPresets> allFoods = new();

    public FoodManagerViewModel()
    {
        _ = LoadFoods();
    }


    partial void OnPageChanged(int value)
    {
        FilterFoods();
    }

    partial void OnSearchTextChanged(string value)
    {
        Page = 0;
        FilterFoods();
    }

    private async Task LoadFoods()
    {
        using var db = new UserContext();
        allFoods = await db.FoodPresets.Where(f => !f.IsDeleted).OrderBy(x => x.Name).ToListAsync();
        FilterFoods();
    }

    private void FilterFoods()
    {
        var filtered = allFoods;
        
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            filtered = filtered.Where(f => f.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();
        }

        var paginated = filtered.Skip(Page * size).Take(size).ToList();
        Foods = new ObservableCollection<FoodPresets>(paginated);
    }

    private void SetupEditFoodDialog(EditFoodDialogViewModel dialogViewModel)
    {
        dialogViewModel.OnSaved += async (preset) =>
        {
            await LoadFoods();
            IsEditFoodDialogVisible = false;
            EditFoodDialogViewModel = null;
        };
        dialogViewModel.OnCancel += () =>
        {
            IsEditFoodDialogVisible = false;
            EditFoodDialogViewModel = null;
        };
        IsEditFoodDialogVisible = true;
    }

    [RelayCommand]
    private void ShowAddFoodDialog()
    {
        EditFoodDialogViewModel = new EditFoodDialogViewModel();
        SetupEditFoodDialog(EditFoodDialogViewModel);
    }

    [RelayCommand]
    private void ShowEditFoodDialog(FoodPresets preset)
    {
        EditFoodDialogViewModel = new EditFoodDialogViewModel(preset);
        SetupEditFoodDialog(EditFoodDialogViewModel);
    }

    [RelayCommand]
    private void HideEditFoodDialog()
    {
        IsEditFoodDialogVisible = false;
        EditFoodDialogViewModel = null;
    }

    [RelayCommand]
    private async Task Remove(FoodPresets preset)
    {
        using var db = new UserContext();
        var presetEntity = await db.FoodPresets.FindAsync(preset.Id);
        if (presetEntity != null)
        {
            presetEntity.IsDeleted = true;
            presetEntity.Updated = DateTime.UtcNow;
            db.FoodPresets.Update(presetEntity);
            await db.SaveChangesAsync();
        }
        await LoadFoods();
    }

    private void ChangePage(int offset)
    {
        if (offset < 0 && Page > 0)
        {
            Page--;
        }
        else if (offset > 0)
        {
            var totalPages = (int)Math.Ceiling((double)allFoods.Count / size);
            if (Page < totalPages - 1)
            {
                Page++;
            }
        }
    }

    [RelayCommand]
    private void PreviousPage()
    {
        ChangePage(-1);
    }

    [RelayCommand]
    private void NextPage()
    {
        ChangePage(1);
    }
}
