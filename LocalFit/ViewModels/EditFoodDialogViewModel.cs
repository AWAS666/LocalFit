using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalFit.Lib.DataBase;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LocalFit.ViewModels;

public partial class EditFoodDialogViewModel : ObservableObject
{
    [ObservableProperty]
    private int? id;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private decimal calories;

    [ObservableProperty]
    private decimal fat;

    [ObservableProperty]
    private decimal protein;

    [ObservableProperty]
    private decimal carbs;

    [ObservableProperty]
    private decimal sugar;

    [ObservableProperty]
    private FoodUnit unit = FoodUnit.Gram100;

    public FoodUnit[] Units => Enum.GetValues<FoodUnit>();

    public event Action<FoodPresets>? OnSaved;
    public event Action? OnCancel;

    public EditFoodDialogViewModel()
    {
    }

    public EditFoodDialogViewModel(FoodPresets preset)
    {
        Id = preset.Id;
        Name = preset.Name;
        Calories = preset.Calories;
        Fat = preset.Fat;
        Protein = preset.Protein;
        Carbs = preset.Carbs;
        Sugar = preset.Sugar;
        Unit = preset.Unit;
    }

    [RelayCommand]
    private async Task Save()
    {
        using var db = new UserContext();
        FoodPresets? preset;
        
        if (Id.HasValue)
        {
            preset = await db.FoodPresets.FindAsync(Id.Value);
            if (preset == null) 
            {
                OnCancel?.Invoke();
                return;
            }
            preset.Name = Name;
            preset.Calories = Calories;
            preset.Fat = Fat;
            preset.Protein = Protein;
            preset.Carbs = Carbs;
            preset.Sugar = Sugar;
            preset.Unit = Unit;
            preset.Updated = DateTime.UtcNow;
            db.FoodPresets.Update(preset);
        }
        else
        {
            preset = new FoodPresets
            {
                Name = Name,
                Calories = Calories,
                Fat = Fat,
                Protein = Protein,
                Carbs = Carbs,
                Sugar = Sugar,
                Unit = Unit,
            };
            db.FoodPresets.Add(preset);
        }
        
        await db.SaveChangesAsync();
        OnSaved?.Invoke(preset);
    }

    [RelayCommand]
    private void Cancel()
    {
        OnCancel?.Invoke();
    }
}
