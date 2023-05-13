using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CrossplatformRadioApp.Views;

public partial class FreqControlPage : UserControl
{
    public FreqControlPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}