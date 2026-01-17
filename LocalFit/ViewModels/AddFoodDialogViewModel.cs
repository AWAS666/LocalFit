using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalFit.Lib;
using LocalFit.Lib.DataBase;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LocalFit.ViewModels;

public partial class AddFoodDialogViewModel : DialogViewModel
{
    [ObservableProperty]
    private string searchText = string.Empty;

    [ObservableProperty]
    private ObservableCollection<FoodPresets> filteredFoods = new();

    [ObservableProperty]
    private FoodPresets? selectedFood;

    [ObservableProperty]
    private decimal? amount = 100;

    [ObservableProperty]
    private decimal? calculatedCalories;

    [ObservableProperty]
    private decimal? calculatedProtein;

    [ObservableProperty]
    private decimal? calculatedFat;

    [ObservableProperty]
    private decimal? calculatedCarbs;

    public decimal Increment => FoodUnitHelper.GetIncrementForUnit(SelectedFood?.Unit);

    [ObservableProperty]
    private bool isEditFoodDialogVisible;

    [ObservableProperty]
    private EditFoodDialogViewModel? editFoodDialogViewModel;

    private List<FoodPresets> allFoods = new();

    public event Action<FoodPresets, decimal>? OnFoodSelected;

    public AddFoodDialogViewModel()
    {
        _ = LoadFoods();
    }

    partial void OnSearchTextChanged(string value)
    {
        FilterFoods();
    }

    private void CalculateNutrition()
    {
        if (SelectedFood == null || Amount == null)
        {
            CalculatedCalories = null;
            CalculatedProtein = null;
            CalculatedFat = null;
            CalculatedCarbs = null;
            return;
        }

        var multiplier = DayTracker.GetMultiplier(SelectedFood.Unit, Amount.Value);

        CalculatedCalories = SelectedFood.Calories * multiplier;
        CalculatedProtein = SelectedFood.Protein * multiplier;
        CalculatedFat = SelectedFood.Fat * multiplier;
        CalculatedCarbs = SelectedFood.Carbs * multiplier;
    }

    private async Task LoadFoods()
    {
        using var db = new UserContext();
        allFoods = await db.GetActiveFoodPresetsAsync(orderByUseCount: true);
        FilterFoods();
    }

    private void FilterFoods()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            FilteredFoods = new ObservableCollection<FoodPresets>(allFoods);
        }
        else
        {
            var filtered = allFoods
                .Where(f => f.Name.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                .OrderByDescending(x => x.UseCount)
                .ThenBy(x => x.Name)
                .ToList();
            FilteredFoods = new ObservableCollection<FoodPresets>(filtered);
        }
    }

    private bool CanSelectFood(FoodPresets? food)
    {
        return food != null && Amount.HasValue;
    }

    [RelayCommand(CanExecute = nameof(CanSelectFood))]
    private void SelectFood(FoodPresets? food)
    {
        OnFoodSelected?.Invoke(food!, Amount!.Value);
    }

    partial void OnAmountChanged(decimal? value)
    {
        CalculateNutrition();
        SelectFoodCommand.NotifyCanExecuteChanged();
    }

    partial void OnSelectedFoodChanged(FoodPresets? value)
    {
        if (value != null)
        {
            // Use LastAmount if available, otherwise default based on unit
            Amount = value.LastAmount ?? (value.Unit == FoodUnit.Piece ? 1 : 100);
            CalculateNutrition();
        }
        SelectFoodCommand.NotifyCanExecuteChanged();
        OnPropertyChanged(nameof(Increment));
    }


    private void SetupEditFoodDialog(EditFoodDialogViewModel dialogViewModel)
    {
        dialogViewModel.OnSaved += async (preset) =>
        {
            await LoadFoods();
            SelectedFood = preset;
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
    private void ShowCreateNewFoodDialog()
    {
        EditFoodDialogViewModel = new EditFoodDialogViewModel();
        SetupEditFoodDialog(EditFoodDialogViewModel);
    }

    [RelayCommand]
    private void HideEditFoodDialog()
    {
        IsEditFoodDialogVisible = false;
        EditFoodDialogViewModel = null;
    }
}
