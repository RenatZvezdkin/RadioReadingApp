using Avalonia.Controls;

namespace CrossplatformRadioApp.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        CC.Content = new FilesPage(this);
    }
}