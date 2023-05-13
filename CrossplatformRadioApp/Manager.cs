using System.IO;
using System.Reflection;
using Avalonia.Controls;
using CrossplatformRadioApp.Views;

namespace CrossplatformRadioApp;

public class Manager
{
    private UserControl _selectedPage;
    private MainWindow _mainWindow;
    public MainWindow MainWindow => _mainWindow;
    public UserControl SelectedPage
    {
        get => _selectedPage;
        set
        {
            _selectedPage = value;
            _mainWindow.CC.Content = value;
        }
    }
    public void InitMainWindow(MainWindow window) => _mainWindow = window;
    private string _getExecutingPath { get => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\..\")); }
    public string ConnectionString => File.ReadAllText(_getExecutingPath+"connectionstring.txt");
    public static Manager Instance { get; } = new();
    
}