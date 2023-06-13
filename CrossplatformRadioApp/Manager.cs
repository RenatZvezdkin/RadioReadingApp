using System.IO;
using System.Linq;
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
            _mainWindow.MainContentControl.Content = value;
        }
    }
    public string GoUpByDirectory(string path, int foldersToGoUp = 1)
        => foldersToGoUp<=1? Directory.GetParent(path).ToString() : GoUpByDirectory(Directory.GetParent(path).ToString(),foldersToGoUp-1);
    public void InitMainWindow(MainWindow window) => _mainWindow = window;
    private string _getExecutingPath => 
        GoUpByDirectory(Assembly.GetExecutingAssembly().Location, 4)+"/";
    private string[] _settingsLines => 
        File.ReadAllLines(_getExecutingPath + "settings.txt");
    private string _getPropertyFromSettings(string prop) =>
        _settingsLines.First(l => l.StartsWith(prop + ":")).Remove(0,prop.Length+1).Trim();
    public string ConnectionString => _getPropertyFromSettings("connectionstring");
    public string DatabaseVersion => _getPropertyFromSettings("databaseversion");
    public static Manager Instance { get; } = new();
    
}