using System;
using System.IO;
using System.Reflection;
using Avalonia.Controls;
using CrossplatformRadioApp.Context;
using CrossplatformRadioApp.Views;

namespace CrossplatformRadioApp;

public class Manager
{
    /// <summary>
    /// Отражает зашифрованный файл для настроек
    /// </summary>
    public ISettingsFile Settings { get; }

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
    /// Выбранная страница основного окна. При присваивании переносит основное окно на эту страницу
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
    /// <summary>
    /// Строка подключения к базе данных MariaDB/MySQL
    /// </summary>
    public string? ConnectionString =>
        Settings.GetValueFromProperty("connectionstring");
    //_getPropertyFromSettings("connectionstring");
    /// <summary>
    /// Версия базы данных из файла настроек
    /// </summary>
    public string? DatabaseVersion =>
        Settings.GetValueFromProperty("databaseversion");
    //_getPropertyFromSettings("databaseversion");
    /// <summary>
    /// Ссылка на единственное значение Manager
    /// </summary>
    public static Manager Instance { get; } = new();
    /// <summary>
    /// true, если подключение с базой данных существует, иначе - false
    /// </summary>
    public bool ConnectedToDatabase {
        get
        {
            if (ConnectionString==null || DatabaseVersion==null)
                return false;
            using var db = new MyDbContext();
            return db.Database.CanConnect();
        }
    }
    /// <summary>
    /// true, если в папке с исполнительным файлом имеется RtlSdrLib (Ссылка на скачивание <a href="https://ftp.osmocom.org/binaries/windows/rtl-sdr/">тут</a>) и количество устройств выше 0, иначе false
    /// </summary>
    public bool SDRsArePresent
    {
        get
        {
            var res = true;
            try{ var devices = RtlSdrManager.RtlSdrDeviceManager.Instance.Devices; }
            catch (Exception ignored) { res = false; }
            return res;
        }
    }
    private Manager()
    {
        Settings = new SettingsFile(GoUpByDirectory(Assembly.GetExecutingAssembly().Location));
    }
}