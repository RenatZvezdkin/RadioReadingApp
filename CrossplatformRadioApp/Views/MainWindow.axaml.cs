using Avalonia.Controls;

namespace CrossplatformRadioApp.Views;
/// <summary>
/// Основное окно, в котором происходит действие
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Manager.Instance.InitMainWindow(this);
        MainContentControl.Content = new MainPage();
    }
}