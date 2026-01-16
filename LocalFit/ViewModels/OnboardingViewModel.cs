using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalFit.Lib.DataBase;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LocalFit.ViewModels;

public partial class OnboardingViewModel : ObservableObject
{
    private readonly MainViewModel mainViewModel;

    [ObservableProperty]
    private int currentStep = 1;

    [ObservableProperty]
    private bool isWeightDialogVisible;

    [ObservableProperty]
    private bool isGoalDialogVisible;

    [ObservableProperty]
    private AddWeightDialogViewModel? addWeightDialogViewModel;

    [ObservableProperty]
    private GoalSetupDialogViewModel? goalSetupDialogViewModel;

    public bool NeedsWeight { get; private set; }
    public bool NeedsGoal { get; private set; }
    public bool ShowGoalStep => NeedsGoal && !NeedsWeight;
    public bool IsComplete => !NeedsWeight && !NeedsGoal;

    public OnboardingViewModel(MainViewModel mainViewModel)
    {
        this.mainViewModel = mainViewModel;
        CheckRequirements();
    }

    private void CheckRequirements()
    {
        using var db = new UserContext();
        var hasWeight = db.Weight.Any();
        var hasGoal = db.Goal.Any();
        
        NeedsWeight = !hasWeight;
        NeedsGoal = !hasGoal;
        
        OnPropertyChanged(nameof(NeedsWeight));
        OnPropertyChanged(nameof(NeedsGoal));
        OnPropertyChanged(nameof(ShowGoalStep));
        OnPropertyChanged(nameof(IsComplete));
        
        if (NeedsWeight)
        {
            CurrentStep = 1;
        }
        else if (NeedsGoal)
        {
            CurrentStep = 2;
        }
        else
        {
            CurrentStep = 3; // Complete
        }
    }

    [RelayCommand]
    private void ShowWeightDialog()
    {
        AddWeightDialogViewModel = new AddWeightDialogViewModel();
        AddWeightDialogViewModel.OnSaved += (weight) =>
        {
            NeedsWeight = false;
            IsWeightDialogVisible = false;
            AddWeightDialogViewModel = null;
            CheckNextStep();
        };
        AddWeightDialogViewModel.OnCancel += () =>
        {
            IsWeightDialogVisible = false;
            AddWeightDialogViewModel = null;
        };
        IsWeightDialogVisible = true;
    }

    [RelayCommand]
    private void ShowGoalDialog()
    {
        GoalSetupDialogViewModel = new GoalSetupDialogViewModel();
        GoalSetupDialogViewModel.OnSaved += (goal) =>
        {
            NeedsGoal = false;
            IsGoalDialogVisible = false;
            GoalSetupDialogViewModel = null;
            CheckNextStep();
        };
        GoalSetupDialogViewModel.OnCancel += () =>
        {
            IsGoalDialogVisible = false;
            GoalSetupDialogViewModel = null;
        };
        IsGoalDialogVisible = true;
    }

    [RelayCommand]
    private void SkipWeight()
    {
        NeedsWeight = false;
        CheckNextStep();
    }

    [RelayCommand]
    private void SkipGoal()
    {
        NeedsGoal = false;
        CheckNextStep();
    }

    private void CheckNextStep()
    {
        OnPropertyChanged(nameof(NeedsWeight));
        OnPropertyChanged(nameof(NeedsGoal));
        OnPropertyChanged(nameof(ShowGoalStep));
        OnPropertyChanged(nameof(IsComplete));
        
        if (NeedsWeight)
        {
            CurrentStep = 1;
        }
        else if (NeedsGoal)
        {
            CurrentStep = 2;
        }
        else
        {
            CurrentStep = 3;
            // Auto-complete after a short delay to show completion message
            Task.Delay(2000).ContinueWith(_ =>
            {
                mainViewModel.ChangeView(new DayTrackerViewModel(mainViewModel));
            });
        }
    }
}
