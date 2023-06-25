using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;

namespace CrossplatformRadioApp.Views;

public partial class MainPage : UserControl
{
    /// <summary>
    /// Главная страница, приветствующая пользователя при входе в программу
    /// </summary>
    public MainPage()
    {
        InitializeComponent();
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
    private void Control_OnLoaded(object? sender, RoutedEventArgs e)
    {
        if (Manager.Instance.ConnectionString==null || Manager.Instance.DatabaseVersion==null || !Manager.Instance.ConnectedToDatabase)
        {
            Manager.Instance.Settings.Recreate();
            var cw = new ConfigWindow();
            cw.ShowDialog(Manager.Instance.MainWindow);
        }
    }
}