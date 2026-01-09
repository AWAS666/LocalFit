using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LocalFit.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    [ObservableProperty]
    private object currentView = new BMRViewModel();

    [RelayCommand]
    public void View1()
    {
        CurrentView = new BMRViewModel();
    }

    [RelayCommand]
    public void View2()
    {
        CurrentView = new FoodManagerViewModel();
    }

    [RelayCommand]
    public void View3()
    {
        CurrentView = null;
    }
}
