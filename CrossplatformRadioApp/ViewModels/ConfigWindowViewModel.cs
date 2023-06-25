using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Platform;
using CrossplatformRadioApp.Models;
using CrossplatformRadioApp.Views;
using MessageBox.Avalonia.Enums;
using MySqlConnector;

namespace CrossplatformRadioApp.ViewModels;

public class ConfigWindowViewModel : INotifyPropertyChanged
{
    public string ConnString { get; set; }
    public string DatabaseName { get; set; }
    public ObservableCollection<string> DatabasesComboboxItems { get; }
    private string _selectedDatabase;
    public RelayCommand ScaryCommandToCheckDatabase{ get; }
    public ConfigWindow Window;
    public string SelectedDatabase
    {
        get => _selectedDatabase;
        set
        {
            _selectedDatabase = value;
        }
    }
    
    public ConfigWindowViewModel()
    {
        ConnString = "";
        DatabaseName = "RadioRecords";
        DatabasesComboboxItems = new ObservableCollection<string>(new[] { "mariadb", "mysql" });
        ScaryCommandToCheckDatabase = new RelayCommand(o =>
        {
            try
            {
                
                using var con = new MySqlConnection(ConnString);
                using var reader = new StreamReader(AssetLoader.Open(new Uri($"avares://{Assembly.GetCallingAssembly().GetName().Name}/Assets/databasescript.sql")));
                using var command = new MySqlCommand(reader.ReadToEnd().Replace("DB_NAME", $"`{DatabaseName}`"),con);
                con.Open();
                if (con.State==ConnectionState.Open)
                {
                    Manager.Instance.Settings.AddProperty("connectionstring", $"database={DatabaseName};{ConnString}");
                    Manager.Instance.Settings.AddProperty("databaseversion", con.ServerVersion+"-"+SelectedDatabase);
                    command.ExecuteNonQuery();
                    Window.Close();
                }
            }
            catch (Exception e)
            {
                MessageBox.Avalonia.MessageBoxManager.
                    GetMessageBoxStandardWindow("Подключение", "Во время подключения произошла ошибка, перепроверьте данные").
                    Show(Window);
            }
        });
        SelectedDatabase = DatabasesComboboxItems.First();
        ConnString = "server=localhost; port=3306; user=root?; password=123456?";
    }
    public event PropertyChangedEventHandler? PropertyChanged;

    protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}