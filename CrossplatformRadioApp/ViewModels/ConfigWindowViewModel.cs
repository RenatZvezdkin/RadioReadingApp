using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using CrossplatformRadioApp.Models;
using CrossplatformRadioApp.Views;
using MySqlConnector;

namespace CrossplatformRadioApp.ViewModels;

public class ConfigWindowViewModel : INotifyPropertyChanged
{
    public string ConnString { get; set; } = "";
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
        DatabasesComboboxItems = new ObservableCollection<string>(new[] { "mariadb", "mysql" });
        ScaryCommandToCheckDatabase = new RelayCommand(o =>
        {
            using var con = new MySqlConnection(ConnString);
            try
            {
                con.Open();
                if (con.State==ConnectionState.Open)
                {
                    Manager.Instance.Settings.AddProperty("connectionstring", "database=RadioRecords;"+ConnString);
                    Manager.Instance.Settings.AddProperty("databaseversion", con.ServerVersion+"-"+SelectedDatabase);
                        //using MySqlCommand command = new MySqlCommand("command lol",con);
                    //command.ExecuteNonQuery();
                    Window.Close();
                }
            }
            catch (Exception e)
            {
                using var task = MessageBox.Avalonia.MessageBoxManager.
                    GetMessageBoxStandardWindow("Подключение", "Во время подключения произошла ошибка, перепроверьте данные").
                    ShowDialog(Window);
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