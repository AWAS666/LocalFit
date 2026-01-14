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
            var onboarding = new OnboardingViewModel();
            onboarding.OnComplete += () =>
            {
                CurrentView = new DayTrackerViewModel();
            };
            CurrentView = onboarding;
        }
        else
        {
            CurrentView = new DayTrackerViewModel();
        }
    }

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
