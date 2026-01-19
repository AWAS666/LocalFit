using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalFit.Extensions;
using LocalFit.Helpers;
using LocalFit.Lib;
using LocalFit.Lib.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
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

    private readonly List<EnumDisplayItem<CalorieGoal>> _goals = EnumHelper.GetDisplayItems<CalorieGoal>().ToList();
    private readonly List<EnumDisplayItem<FitnessLevel>> _fitnessLevels = EnumHelper.GetDisplayItems<FitnessLevel>().ToList();
    private readonly List<EnumDisplayItem<Sex>> _sexes = EnumHelper.GetDisplayItems<Sex>().ToList();
    private readonly List<EnumDisplayItem<DietType>> _dietTypes = EnumHelper.GetDisplayItems<DietType>().ToList();

    public IEnumerable<EnumDisplayItem<CalorieGoal>> Goals => _goals;
    public IEnumerable<EnumDisplayItem<FitnessLevel>> FitnessLevels => _fitnessLevels;
    public IEnumerable<EnumDisplayItem<Sex>> Sexes => _sexes;
    public IEnumerable<EnumDisplayItem<DietType>> DietTypes => _dietTypes;

    public string FitnessLevelDescription => this.FitnessLevel.GetDescription();
    
    private EnumDisplayItem<CalorieGoal>? _selectedGoalItem;
    private EnumDisplayItem<FitnessLevel>? _selectedFitnessLevelItem;
    private EnumDisplayItem<Sex>? _selectedSexItem;
    private EnumDisplayItem<DietType>? _selectedDietTypeItem;

    public EnumDisplayItem<CalorieGoal>? SelectedGoalItem
    {
        get => _selectedGoalItem;
        set
        {
            if (SetProperty(ref _selectedGoalItem, value) && value != null)
            {
                Goal = value.Value;
            }
        }
    }

    public EnumDisplayItem<FitnessLevel>? SelectedFitnessLevelItem
    {
        get => _selectedFitnessLevelItem;
        set
        {
            if (SetProperty(ref _selectedFitnessLevelItem, value) && value != null)
            {
                FitnessLevel = value.Value;
            }
        }
    }

    public EnumDisplayItem<Sex>? SelectedSexItem
    {
        get => _selectedSexItem;
        set
        {
            if (SetProperty(ref _selectedSexItem, value) && value != null)
            {
                Sex = value.Value;
            }
        }
    }

    public EnumDisplayItem<DietType>? SelectedDietTypeItem
    {
        get => _selectedDietTypeItem;
        set
        {
            if (SetProperty(ref _selectedDietTypeItem, value) && value != null)
            {
                DietType = value.Value;
            }
        }
    }

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

        UpdateSelectedItems();
    }

    private void UpdateSelectedItems()
    {
        SelectedGoalItem = _goals.FirstOrDefault(g => g.Value == Goal);
        SelectedFitnessLevelItem = _fitnessLevels.FirstOrDefault(f => f.Value == FitnessLevel);
        SelectedSexItem = _sexes.FirstOrDefault(s => s.Value == Sex);
        SelectedDietTypeItem = _dietTypes.FirstOrDefault(d => d.Value == DietType);
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
        SelectedFitnessLevelItem = _fitnessLevels.FirstOrDefault(f => f.Value == value);
    }

    partial void OnGoalChanged(CalorieGoal value)
    {
        SelectedGoalItem = _goals.FirstOrDefault(g => g.Value == value);
    }

    partial void OnSexChanged(Sex value)
    {
        SelectedSexItem = _sexes.FirstOrDefault(s => s.Value == value);
    }

    partial void OnDietTypeChanged(DietType value)
    {
        SelectedDietTypeItem = _dietTypes.FirstOrDefault(d => d.Value == value);
    }
}
