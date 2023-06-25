using System.IO;
using System.Linq;
using System.Reflection;
using Avalonia.Controls;
using CrossplatformRadioApp.Context;
using CrossplatformRadioApp.Views;

namespace CrossplatformRadioApp;

public class Manager
{
    private UserControl _selectedPage;
    /// <summary>
    /// Основное окно, на котором должно происходить основное действие
    /// </summary>
    public MainWindow MainWindow { get; private set; }
    /// <summary>
    /// Задает значение основному окну. Отделено от <see cref="MainWindow"/>, чтобы показывать, что человек это делает намеренно
    /// </summary>
    /// <param name="window">основное окно</param>
    public void InitMainWindow(MainWindow window) => MainWindow = window;
    /// <summary>
    /// Выбранная страница основного окна, при присваивании переносит основное окно на эту страницу
    /// </summary>
    public UserControl SelectedPage
    {
        get => _selectedPage;
        set
        {
            _selectedPage = value;
            MainWindow.MainContentControl.Content = value;
        }
    }
    /// <summary>
    /// Возвращает путь, выше на определенное количество уровней
    /// </summary>
    /// <param name="path">изначальный путь</param>
    /// <param name="foldersToGoUp">количество уровней, чтобы подняться, должно быть больше 0</param>
    /// <returns></returns>
    public string GoUpByDirectory(string path, int foldersToGoUp = 1)
        => foldersToGoUp<=1? Directory.GetParent(path).ToString() : GoUpByDirectory(Directory.GetParent(path).ToString(),foldersToGoUp-1);
    private string _getExecutingPath => 
        GoUpByDirectory(Assembly.GetExecutingAssembly().Location, 4)+"/";
    private string[] _settingsLines => 
        File.ReadAllLines(_getExecutingPath + "settings.txt");
    private string _getPropertyFromSettings(string prop) =>
        _settingsLines.First(l => l.StartsWith(prop + ":")).Remove(0,prop.Length+1).Trim();
    /// <summary>
    /// возвращает строку подключения к базе данных MariaDB/MySQL
    /// </summary>
    public string ConnectionString => _getPropertyFromSettings("connectionstring");
    /// <summary>
    /// возвращает версию базы данных из файла настроек
    /// </summary>
    public string DatabaseVersion => _getPropertyFromSettings("databaseversion");
    /// <summary>
    /// возвращает ссылку на значение Manager
    /// </summary>
    public static Manager Instance { get; } = new();
    /// <summary>
    /// true, если подключение с базой данных существует, иначе - false
    /// </summary>
    public bool Connected {
        get
        {
            using var db = new MyDbContext();
            return db.Database.CanConnect();
        }
    }
    private Manager()
    {
    }
}