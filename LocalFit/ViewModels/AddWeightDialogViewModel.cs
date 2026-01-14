using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalFit.Lib.DataBase;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LocalFit.ViewModels;

public partial class AddWeightDialogViewModel : ObservableObject
{
    [ObservableProperty]
    private decimal weight = 70;

    [ObservableProperty]
    private decimal height = 175;

    [ObservableProperty]
    private string? note;

    public event Action<WeightTracker>? OnSaved;
    public event Action? OnCancel;

    public AddWeightDialogViewModel()
    {
        LoadLatestWeight();
    }

    private void LoadLatestWeight()
    {
        using var db = new UserContext();
        var latest = db.Weight.OrderByDescending(x => x.Created).FirstOrDefault();
        if (latest != null)
        {
            Weight = latest.Weight;
            Height = latest.Height;
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        using var db = new UserContext();
        var entry = new WeightTracker
        {
            Weight = Weight,
            Height = Height,
            Note = Note
        };
        db.Weight.Add(entry);
        await db.SaveChangesAsync();
        OnSaved?.Invoke(entry);
    }

    [RelayCommand]
    private void Cancel()
    {
        OnCancel?.Invoke();
    }
}
