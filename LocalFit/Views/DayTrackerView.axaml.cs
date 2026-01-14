using Avalonia.Controls;
using Avalonia.Input;
using LocalFit.ViewModels;

namespace LocalFit.Views;

public partial class DayTrackerView : UserControl
{
    public DayTrackerView()
    {
        InitializeComponent();
    }

    private void OnBackdropTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is DayTrackerViewModel vm)
        {
            if (vm.IsAddFoodDialogVisible)
                vm.HideAddFoodDialogCommand.Execute(null);
            else if (vm.IsEditEatenFoodDialogVisible)
                vm.HideEditEatenFoodDialogCommand.Execute(null);
        }
    }
}