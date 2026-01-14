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
            vm.HideAddFoodDialogCommand.Execute(null);
        }
    }
}