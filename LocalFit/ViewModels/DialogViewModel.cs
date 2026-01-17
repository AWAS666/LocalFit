using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;

namespace LocalFit.ViewModels;

/// <summary>
/// Base class for dialog ViewModels that provides common event handling for save and cancel actions.
/// </summary>
public abstract partial class DialogViewModel : ObservableObject
{
    public event Action? OnCancel;

    [RelayCommand]
    protected void Cancel()
    {
        OnCancel?.Invoke();
    }
}

/// <summary>
/// Base class for dialog ViewModels that return a result when saved.
/// </summary>
public abstract partial class DialogViewModel<TResult> : DialogViewModel
{
    public event Action<TResult>? OnSaved;

    protected void InvokeSaved(TResult result)
    {
        OnSaved?.Invoke(result);
    }
}
