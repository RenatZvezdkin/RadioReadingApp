using System;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using CrossplatformRadioApp.ViewModels;

namespace CrossplatformRadioApp.Views;

public partial class ComboboxMessageBox : Window
{
    public ComboboxMessageBox(IEnumerable<object> elementsToChoose, string shownProperty ,string message, Action<object?> actionAfterChoose)
    {
        InitializeComponent();
        DataContext = new MessageBoxViewModel(this, elementsToChoose, message, shownProperty, actionAfterChoose);
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    private void ElementsCb_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        (DataContext as MessageBoxViewModel).SelectedItem = (sender as ComboBox)?.SelectedItem;
        (DataContext as MessageBoxViewModel)?.RaiseUpdate();
    }
}