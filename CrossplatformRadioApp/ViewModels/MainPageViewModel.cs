using Avalonia.Controls;
using CrossplatformRadioApp.Models;
using CrossplatformRadioApp.Views;

namespace CrossplatformRadioApp.ViewModels;

public class MainPageViewModel: ViewModelBase
{
    public RelayCommand MoveToSavedFilesCommand { get; }
    public RelayCommand MoveToFreqControlCommand { get; }
    public MainPageViewModel()
    {
        MoveToSavedFilesCommand = new RelayCommand(o => Manager.Instance.SelectedPage = new FilesPage());
        MoveToFreqControlCommand = new RelayCommand(o => Manager.Instance.SelectedPage = new FreqControlPage());
    }
}