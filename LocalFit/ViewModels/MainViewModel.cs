using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LocalFit.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private object currentView = new DayTrackerViewModel();

    [RelayCommand]
    public void NavigateToFoodManager()
    {
        CurrentView = new FoodManagerViewModel();
    }

    [RelayCommand]
    public void NavigateToDayTracker()
    {
        CurrentView = new DayTrackerViewModel();
    }

    [RelayCommand]
    public void NavigateToWeightTracker()
    {
        CurrentView = new WeightTrackerViewModel();
    }

    [RelayCommand]
    public void NavigateToSettings()
    {
        CurrentView = new SettingsViewModel();
    }
}
