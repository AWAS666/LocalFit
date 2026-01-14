using Avalonia.Controls;
using Avalonia.Input;
using LocalFit.ViewModels;

namespace LocalFit.Views;

public partial class SettingsView : UserControl
{
    public SettingsView()
    {
        InitializeComponent();
    }

    private void OnBackdropTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is SettingsViewModel vm)
        {
            vm.HideGoalSetupDialogCommand.Execute(null);
        }
    }
}
