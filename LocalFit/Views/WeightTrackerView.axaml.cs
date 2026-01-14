using Avalonia.Controls;
using Avalonia.Input;
using LocalFit.ViewModels;

namespace LocalFit.Views;

public partial class WeightTrackerView : UserControl
{
    public WeightTrackerView()
    {
        InitializeComponent();
    }

    private void OnBackdropTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is WeightTrackerViewModel vm)
        {
            vm.HideAddWeightDialogCommand.Execute(null);
        }
    }
}