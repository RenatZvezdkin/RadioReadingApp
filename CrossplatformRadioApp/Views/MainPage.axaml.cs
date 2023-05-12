using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CrossplatformRadioApp.Views;

public partial class MainPage : UserControl
{
    private Window _parnetWindow;
    public MainPage()
    {
        InitializeComponent();
    }
    public MainPage(Window parentWindow)
    {
        InitializeComponent();
        _parnetWindow = parentWindow;
    }
    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}