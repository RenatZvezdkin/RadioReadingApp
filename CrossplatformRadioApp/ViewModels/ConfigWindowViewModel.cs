using System;
using System.Collections.ObjectModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;
using Avalonia.Platform;
using CrossplatformRadioApp.Models;
using CrossplatformRadioApp.Views;
using MySqlConnector;

namespace CrossplatformRadioApp.ViewModels;

public class ConfigWindowViewModel : ViewModelBase
{
    public string ConnString { get; set; }
    public string DatabaseName { get; set; }
    public ObservableCollection<string> DatabasesComboboxItems { get; }
    public RelayCommand ScaryCommandToCheckDatabase{ get; }
    public ConfigWindow Window;
    public string SelectedDatabase { get; set; }

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
                    if (!Manager.Instance.SDRsArePresent)
                        MsBox.Avalonia.MessageBoxManager.
                        GetMessageBoxStandard("RtlSdrLib", "В проекте отсутствует RtlSdrLib.dll и его зависимости. Пока они не будут добавлены функция записи частот не будет доступна. Скачать их вы можете на \"ftp.osmocom.org\"").
                        ShowWindowDialogAsync(Window);
                    Window.Close();
                }
            }
            catch (Exception e)
            {
                MsBox.Avalonia.MessageBoxManager.
                    GetMessageBoxStandard("Подключение", "Во время подключения произошла ошибка, перепроверьте данные").
                    ShowAsync();
            }
        });
        SelectedDatabase = DatabasesComboboxItems.First();
        ConnString = "server=localhost; port=3306; user=root?; password=123456?";
    }
}