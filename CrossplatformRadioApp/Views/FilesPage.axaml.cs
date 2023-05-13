using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CrossplatformRadioApp.ViewModels;

namespace CrossplatformRadioApp.Views;

public partial class FilesPage : UserControl
{
    public FilesPage()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }
}