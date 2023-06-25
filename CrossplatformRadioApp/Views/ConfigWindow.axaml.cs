using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using CrossplatformRadioApp.ViewModels;

namespace CrossplatformRadioApp.Views;

public partial class ConfigWindow : Window
{
    public ConfigWindow()
    {
        InitializeComponent();
        (DataContext as ConfigWindowViewModel).Window = this;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void TopLevel_OnClosed(object? sender, EventArgs e)
    {
        if (!Manager.Instance.ConnectedToDatabase)
            Manager.Instance.MainWindow.Close();
    }
}