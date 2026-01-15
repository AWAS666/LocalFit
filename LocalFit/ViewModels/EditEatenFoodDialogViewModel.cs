using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalFit.Lib.DataBase;
using System;
using System.Threading.Tasks;

namespace LocalFit.ViewModels;

public partial class EditEatenFoodDialogViewModel : ObservableObject
{
    [ObservableProperty]
    private FoodEaten foodEaten;

    [ObservableProperty]
    private decimal? amount;

    public string FoodName => FoodEaten.Preset.Name;
    public string UnitDisplay => FoodEaten.Preset.Unit == FoodUnit.Gram100 ? "g" : "piece(s)";

    public event Action<FoodEaten>? OnSaved;
    public event Action? OnCancel;

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
            OnCancel?.Invoke();
            return;
        }

        entry.Amount = Amount!.Value;
        entry.Updated = DateTime.UtcNow;
        await db.SaveChangesAsync();

        FoodEaten.Amount = Amount.Value;
        OnSaved?.Invoke(FoodEaten);
    }

    partial void OnAmountChanged(decimal? value)
    {
        SaveCommand.NotifyCanExecuteChanged();
    }

    [RelayCommand]
    private void Cancel()
    {
        OnCancel?.Invoke();
    }
}
