using Avalonia.Controls;

namespace CrossplatformRadioApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainContentControl.Content = new MainPage();
        Manager.Instance.InitMainWindow(this);
    }
}