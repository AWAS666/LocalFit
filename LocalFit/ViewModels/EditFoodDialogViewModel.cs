using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalFit.Lib.DataBase;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LocalFit.ViewModels;

public partial class EditFoodDialogViewModel : DialogViewModel<FoodPresets>
{
    [ObservableProperty]
    private int? id;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private decimal? calories;

    [ObservableProperty]
    private decimal? fat;

    [ObservableProperty]
    private decimal? protein;

    [ObservableProperty]
    private decimal? carbs;

    [ObservableProperty]
    private decimal? sugar;

    [ObservableProperty]
    private FoodUnit unit = FoodUnit.Gram100;

    public FoodUnit[] Units => Enum.GetValues<FoodUnit>();

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

    partial void OnNameChanged(string value)
    {
        SaveCommand.NotifyCanExecuteChanged();
    }

    private bool CanSave()
    {
        return !string.IsNullOrWhiteSpace(Name);
    }

    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task Save()
    {
        using var db = new UserContext();
        FoodPresets? preset;
        
        if (Id.HasValue)
        {
            preset = await db.FoodPresets.FindAsync(Id.Value);
        if (preset == null) 
        {
            Cancel();
            return;
        }
            preset.Name = Name;
            preset.Calories = Calories ?? 0;
            preset.Fat = Fat ?? 0;
            preset.Protein = Protein ?? 0;
            preset.Carbs = Carbs ?? 0;
            preset.Sugar = Sugar ?? 0;
            preset.Unit = Unit;
            preset.Updated = DateTime.UtcNow;
            db.FoodPresets.Update(preset);
        }
        else
        {
            preset = new FoodPresets
            {
                Name = Name,
                Calories = Calories ?? 0,
                Fat = Fat ?? 0,
                Protein = Protein ?? 0,
                Carbs = Carbs ?? 0,
                Sugar = Sugar ?? 0,
                Unit = Unit,
            };
            db.FoodPresets.Add(preset);
        }
        
        await db.SaveChangesAsync();
        InvokeSaved(preset);
    }
}
