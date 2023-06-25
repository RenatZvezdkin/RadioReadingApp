using CrossplatformRadioApp.Models;
using CrossplatformRadioApp.Views;

namespace CrossplatformRadioApp.ViewModels;

public class MainPageViewModel: ViewModelBase
{
    public RelayCommand MoveToSavedFilesCommand { get; }
    public RelayCommand MoveToFreqControlCommand { get; }
    public MainPageViewModel()
    {
        MoveToSavedFilesCommand = new RelayCommand(o => Manager.Instance.SelectedPage = new FilesPage(), o=> Manager.Instance.ConnectedToDatabase);
        MoveToFreqControlCommand = new RelayCommand(o => Manager.Instance.SelectedPage = new FreqControlPage(), o=> Manager.Instance.ConnectedToDatabase);
    }
}