using Avalonia.Controls;
using Avalonia.Markup.Xaml;

namespace CrossplatformRadioApp.Views;

/// <summary>
/// Страница, для управления загруженными файлами
/// </summary>
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