using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalFit.Lib;
using LocalFit.Lib.DataBase;
using System;
using System.Threading.Tasks;

namespace LocalFit.ViewModels;

public partial class EditEatenFoodDialogViewModel : DialogViewModel<FoodEaten>
{
    [ObservableProperty]
    private FoodEaten foodEaten;

    [ObservableProperty]
    private decimal? amount;

    public string FoodName => FoodEaten.Preset.Name;
    public string UnitDisplay => FoodEaten.Preset.Unit == FoodUnit.Gram100 ? "g" : "piece(s)";
    public decimal Increment => FoodUnitHelper.GetIncrementForUnit(FoodEaten.Preset.Unit);

    public EditEatenFoodDialogViewModel(FoodEaten foodEaten)
    {
        FoodEaten = foodEaten;
        Amount = foodEaten.Amount;
    }

    private bool CanSave()
    {
        return Amount.HasValue;
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task Save()
    {
        using var db = new UserContext();
        var entry = await db.FoodEaten.FindAsync(FoodEaten.Id);
        if (entry == null)
        {
            Cancel();
            return;
        }

        // Update last amount on preset
        var preset = await db.FoodPresets.FindAsync(FoodEaten.Preset.Id);
        if (preset != null)
        {
            preset.LastAmount = Amount!.Value;
            preset.Updated = DateTime.UtcNow;
            db.FoodPresets.Update(preset);
        }

        entry.Amount = Amount!.Value;
        entry.Updated = DateTime.UtcNow;
        await db.SaveChangesAsync();

        FoodEaten.Amount = Amount.Value;
        InvokeSaved(FoodEaten);
    }

    partial void OnAmountChanged(decimal? value)
    {
        SaveCommand.NotifyCanExecuteChanged();
    }
}
