using Avalonia.Controls;

namespace CrossplatformRadioApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        CC.Content = new MainPage();
        Manager.Instance.InitMainWindow(this);
    }
}