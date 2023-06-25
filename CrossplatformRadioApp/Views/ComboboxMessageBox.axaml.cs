using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CrossplatformRadioApp.Models;

namespace CrossplatformRadioApp.Views;

public partial class ComboboxMessageBox : Window
{
    private ComboboxMessageBox(IEnumerable<object> elementsToChoose, string shownProperty ,string message, Action<object?> actionAfterChoose)
    {
        InitializeComponent();
        DataContext = new MessageBoxViewModel(this, elementsToChoose, message, shownProperty, actionAfterChoose);
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    /// <summary>
    /// Вызывает окно для выбора элемента из представленных
    /// </summary>
    /// <param name="parentWindow">окно, которое должно быть перекрыто этим</param>
    /// <param name="elementsToChoose">элементы для выбора</param>
    /// <param name="shownProperty">свойство, показывающееся пользователю</param>
    /// <param name="message">сообщение для пользователя</param>
    /// <param name="actionAfterChoose">действие с выбранным элементом, после нажатия кнопки выбора</param>
    public static void ShowDialog(Window parentWindow, IEnumerable<object> elementsToChoose, string shownProperty ,string message, Action<object?> actionAfterChoose)
    {
        new ComboboxMessageBox(elementsToChoose, shownProperty, message, actionAfterChoose).ShowDialog(parentWindow);
    }
}
public class MessageBoxViewModel
{
    public ObservableCollection<ComboboxItemContainer> ComboboxCollection { get; }
    public string Message { get; }
    private ComboboxItemContainer _selectedItem;
    public ComboboxItemContainer? SelectedItem { 
        get =>_selectedItem;
        set
        {
            _selectedItem = value;
            RaiseUpdate();
        } 
    }
    public RelayCommand SelectButtonCommand { get; }
    protected internal MessageBoxViewModel(ComboboxMessageBox window, IEnumerable<object> comboboxItems, string message, string shownProperty, Action<object?> selectionButton)
    {
        ComboboxCollection = new ObservableCollection<ComboboxItemContainer>(comboboxItems.Select(o => new ComboboxItemContainer(o, shownProperty)));
        Message = message;
        SelectButtonCommand = new RelayCommand(o =>
        {
            selectionButton.Invoke(SelectedItem.Value);
            window.Close();
        }, o=> SelectedItem!=null);
        SelectedItem = ComboboxCollection.First();
    }
    private void RaiseUpdate()
    {
        SelectButtonCommand.RaiseCanExecuteChanged();
    }
}

public class ComboboxItemContainer
{
    public object Value { get; }
    public string ShownProperty { get; }
    public ComboboxItemContainer(object value, string shownProperty)
    {
        Value = value;
        ShownProperty = value.GetType().GetProperty(shownProperty)?.GetValue(value, null)?.ToString() ?? "Property Not Found";
    }
}