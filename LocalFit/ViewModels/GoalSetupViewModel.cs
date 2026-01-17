using CommunityToolkit.Mvvm.ComponentModel;
using LocalFit.Lib.DataBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LocalFit.ViewModels;

public partial class GoalSetupViewModel : ObservableObject
{
    [ObservableProperty]
    private FitnessGoal? currentGoal;

    [ObservableProperty]
    private WeightTracker? currentWeight;

    public GoalSetupViewModel()
    {
        using var db = new UserContext();
        CurrentGoal = db.GetLatestGoal();
        CurrentWeight = db.GetLatestWeight();
    }
}
