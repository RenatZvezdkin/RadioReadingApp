using Avalonia.Controls;

namespace CrossplatformRadioApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        Manager.Instance.InitMainWindow(this);
        MainContentControl.Content = new MainPage();
    }
}