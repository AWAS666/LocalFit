using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalFit.Lib.DataBase;
using System.Linq;

namespace LocalFit.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private object currentView = null!;

    public MainViewModel()
    {
        CheckOnboarding();
    }

    private void CheckOnboarding()
    {
        using var db = new UserContext();
        bool needsWeight = !db.Weight.Any();
        bool needsGoal = !db.Goal.Any();

        if (needsWeight || needsGoal)
        {
            CurrentView = new OnboardingViewModel(this);
        }
        else
        {
            CurrentView = new DayTrackerViewModel(this);
        }
    }

    [RelayCommand]
    public void NavigateToFoodManager()
    {
        CurrentView = new FoodManagerViewModel(this);
    }

    [RelayCommand]
    public void NavigateToDayTracker()
    {
        CurrentView = new DayTrackerViewModel(this);
    }

    [RelayCommand]
    public void NavigateToWeightTracker()
    {
        CurrentView = new WeightTrackerViewModel(this);
    }

    public void ChangeView(object view)
    {
        CurrentView = view;
    }

    [RelayCommand]
    public void NavigateToSettings()
    {
        CurrentView = new SettingsViewModel(this);
    }

    [RelayCommand]
    public void NavigateToNutritionCalculator()
    {
        CurrentView = new BMRViewModel(this);
    }
}
