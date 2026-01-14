using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LocalFit.Lib.DataBase;
using System.Linq;
using System.Threading.Tasks;

namespace LocalFit.ViewModels;

public partial class WeightTrackerViewModel : ObservableObject
{
    [ObservableProperty]
    private WeightTracker? current;

    [ObservableProperty]
    private decimal weight; 

    public WeightTrackerViewModel()
    {
        using var db = new UserContext();
        Current = db.Weight.OrderByDescending(x => x.Created).FirstOrDefault();
    }

    [RelayCommand]
    private async Task AddWeight()
    {
        using var db = new UserContext();
        var entry = new WeightTracker
        {
            Weight = Weight,
        };
        db.Weight.Add(entry);
        await db.SaveChangesAsync();
        Current = entry;
    }
}

