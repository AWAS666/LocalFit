using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalFit.Lib.DataBase;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LocalFit.ViewModels;

public partial class WeightTrackerViewModel : ObservableObject
{
    [ObservableProperty]
    private WeightTracker? current;

    [ObservableProperty]
    private ObservableCollection<WeightTracker> weightHistory = new();

    [ObservableProperty]
    private bool isAddWeightDialogVisible;

    [ObservableProperty]
    private AddWeightDialogViewModel? addWeightDialogViewModel;

    public WeightTrackerViewModel(MainViewModel mainViewModel)
    {
        LoadWeights();
    }

    private void LoadWeights()
    {
        using var db = new UserContext();
        Current = db.GetLatestWeight();
        var history = db.Weight.OrderByDescending(x => x.Created).Take(20).ToList();
        WeightHistory = new ObservableCollection<WeightTracker>(history);
    }

    private void SetupAddWeightDialog(AddWeightDialogViewModel dialogViewModel)
    {
        dialogViewModel.OnSaved += (entry) =>
        {
            Current = entry;
            LoadWeights();
            IsAddWeightDialogVisible = false;
            AddWeightDialogViewModel = null;
        };
        dialogViewModel.OnCancel += () =>
        {
            IsAddWeightDialogVisible = false;
            AddWeightDialogViewModel = null;
        };
        IsAddWeightDialogVisible = true;
    }

    [RelayCommand]
    private void ShowAddWeightDialog()
    {
        AddWeightDialogViewModel = new AddWeightDialogViewModel();
        SetupAddWeightDialog(AddWeightDialogViewModel);
    }

    [RelayCommand]
    private void HideAddWeightDialog()
    {
        IsAddWeightDialogVisible = false;
        AddWeightDialogViewModel = null;
    }
}

