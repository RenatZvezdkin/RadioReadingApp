using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Avalonia.Controls.Selection;
using Avalonia.Data;
using CrossplatformRadioApp.Models;
using CrossplatformRadioApp.Views;

namespace CrossplatformRadioApp.ViewModels;

public class MessageBoxViewModel
{
    public ObservableCollection<object> ComboboxCollection { get; }
    public Action<object?> EndingAction { get; }
    public string Message { get; }
    public object? SelectedItem { get; set; }
    public IBinding ShownProperty { get; }
    public RelayCommand SelectButtonCommand { get; }

    public MessageBoxViewModel(ComboboxMessageBox window,IEnumerable<object> comboboxItems, string message, string shownProperty, Action<object?> selectionButton)
    {
        ComboboxCollection = new ObservableCollection<object>(comboboxItems);
        Message = message;
        ShownProperty = new Binding{ElementName = shownProperty};
        SelectButtonCommand = new RelayCommand(o =>
        {
            selectionButton.Invoke(SelectedItem);
            window.Close();
        }, o=> SelectedItem!=null);
    }

    public void RaiseUpdate()
    {
        SelectButtonCommand.RaiseCanExecuteChanged();
    }
}