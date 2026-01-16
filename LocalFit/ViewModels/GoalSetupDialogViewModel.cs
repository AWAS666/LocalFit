using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalFit.Lib;
using LocalFit.Lib.DataBase;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace LocalFit.ViewModels;

public partial class GoalSetupDialogViewModel : ObservableObject
{
    [ObservableProperty]
    private CalorieGoal goal = CalorieGoal.Maintenance;

    [ObservableProperty]
    private FitnessLevel fitnessLevel = FitnessLevel.Sedentary;

    [ObservableProperty]
    private Sex sex = Sex.Male;

    [ObservableProperty]
    private WeightTracker? currentWeight;

    public CalorieGoal[] Goals => Enum.GetValues<CalorieGoal>();
    public FitnessLevel[] FitnessLevels => Enum.GetValues<FitnessLevel>();
    public Sex[] Sexes => Enum.GetValues<Sex>();

    public string FitnessLevelDescription => FitnessLevel.GetDescription();

    public event Action<FitnessGoal>? OnSaved;
    public event Action? OnCancel;

    public GoalSetupDialogViewModel()
    {
        LoadCurrentGoal();
    }

    private void LoadCurrentGoal()
    {
        using var db = new UserContext();
        var currentGoal = db.Goal.OrderByDescending(x => x.Created).FirstOrDefault();
        CurrentWeight = db.Weight.OrderByDescending(x => x.Created).FirstOrDefault();
        
        if (currentGoal != null)
        {
            Goal = currentGoal.Goal;
            FitnessLevel = currentGoal.FitLevel;
            Sex = currentGoal.Sex;
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        using var db = new UserContext();
        var fitnessGoal = new FitnessGoal
        {
            Goal = Goal,
            FitLevel = FitnessLevel,
            Sex = Sex
        };
        db.Goal.Add(fitnessGoal);
        await db.SaveChangesAsync();
        OnSaved?.Invoke(fitnessGoal);
    }

    [RelayCommand]
    private void Cancel()
    {
        OnCancel?.Invoke();
    }

    partial void OnFitnessLevelChanged(FitnessLevel value)
    {
        OnPropertyChanged(nameof(FitnessLevelDescription));
    }
}
