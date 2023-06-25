using System.ComponentModel;
using System.Runtime.CompilerServices;
using ReactiveUI;

namespace CrossplatformRadioApp.ViewModels;

public class ViewModelBase : ReactiveObject
{
    public event PropertyChangedEventHandler? PropertyChanged;
    internal void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}