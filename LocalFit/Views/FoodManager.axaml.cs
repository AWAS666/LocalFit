using Avalonia.Controls;
using Avalonia.Input;
using LocalFit.ViewModels;

namespace LocalFit.Views;

public partial class FoodManager : UserControl
{
    public FoodManager()
    {
        InitializeComponent();
    }

    private void OnBackdropTapped(object? sender, TappedEventArgs e)
    {
        if (DataContext is FoodManagerViewModel vm)
        {
            vm.HideEditFoodDialogCommand.Execute(null);
        }
    }
}