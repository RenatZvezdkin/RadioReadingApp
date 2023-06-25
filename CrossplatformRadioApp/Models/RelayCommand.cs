using System;
using System.Windows.Input;

namespace CrossplatformRadioApp.Models;

public class RelayCommand: ICommand
{
    private readonly Action<object> _execute;
    private readonly Func<object, bool>? _canExecute;
    /// <summary>
    /// Конструктор класса для команды кнопки. Он используется для привязки свойства Command
    /// </summary>
    /// <param name="execute">функция, которая должна быть выполнена при нажатии на кнопку, к которой привязана комманда</param>
    /// <param name="canExecute">условие, которое должно быть соблюдено для разрешения пользования кнопкой и ее коммандой</param>
    /// <exception cref="ArgumentNullException">вызывается при условии, если execute равен null</exception>
    public RelayCommand(Action<object> execute, Func<object, bool>? canExecute = null)
    {
        _execute = execute ?? throw new ArgumentNullException(nameof(execute));
        _canExecute = canExecute;
    }
    public event EventHandler CanExecuteChanged;
    /// <summary>
    /// Проверяет условия для разрешения пользования кнопкой
    /// </summary>
    /// <param name="parameter">объект, для проверки на условия, обычно кнопка</param>
    /// <returns>true, если все условия соблюдены и false, если нет</returns>
    public bool CanExecute(object parameter) => _canExecute == null || _canExecute(parameter);
    public void Execute(object parameter) => _execute(parameter);
    /// <summary>
    /// Перероверяет условия для разрешения пользования коммандой кнопки, и, при их соблюдении, открывает ее
    /// </summary>
    public void RaiseCanExecuteChanged() => CanExecuteChanged?.Invoke(this, EventArgs.Empty);
}