using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalFit.Lib;
using LocalFit.Lib.DataBase;
using System;
using System.Threading.Tasks;

namespace LocalFit.ViewModels;

public partial class GoalSetupDialogViewModel : DialogViewModel<FitnessGoal>
{
    [ObservableProperty]
    private CalorieGoal goal = CalorieGoal.Maintenance;

    [ObservableProperty]
    private FitnessLevel fitnessLevel = FitnessLevel.Sedentary;

    [ObservableProperty]
    private Sex sex = Sex.Male;

    [ObservableProperty]
    private DietType dietType = DietType.Balanced;

    [ObservableProperty]
    private WeightTracker? currentWeight;

    public CalorieGoal[] Goals => Enum.GetValues<CalorieGoal>();
    public FitnessLevel[] FitnessLevels => Enum.GetValues<FitnessLevel>();
    public Sex[] Sexes => Enum.GetValues<Sex>();
    public DietType[] DietTypes => Enum.GetValues<DietType>();

    public string FitnessLevelDescription => FitnessLevel.GetDescription();

    public GoalSetupDialogViewModel()
    {
        LoadCurrentGoal();
    }

    private void LoadCurrentGoal()
    {
        using var db = new UserContext();
        var currentGoal = db.GetLatestGoal();
        CurrentWeight = db.GetLatestWeight();
        
        if (currentGoal != null)
        {
            Goal = currentGoal.Goal;
            FitnessLevel = currentGoal.FitLevel;
            Sex = currentGoal.Sex;
            DietType = currentGoal.DietType;
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
            Sex = Sex,
            DietType = DietType
        };
        db.Goal.Add(fitnessGoal);
        await db.SaveChangesAsync();
        InvokeSaved(fitnessGoal);
    }

    partial void OnFitnessLevelChanged(FitnessLevel value)
    {
        OnPropertyChanged(nameof(FitnessLevelDescription));
    }
}
