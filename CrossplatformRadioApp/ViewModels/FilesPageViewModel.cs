using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Metadata;
using CrossplatformRadioApp.Models;
using MessageBox.Avalonia.Enums;
using ReactiveUI;

namespace CrossplatformRadioApp.ViewModels
{
    class FilesPageViewModel : ViewModelBase, INotifyCollectionChanged
    {
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        private SaveFileDialog _saveFileDialog;
        public ObservableCollection<FileModel> FileModels { get; }
        public List<FileModel>? _selectedFileModels;
        public List<FileModel>? SelectedFileModels
        {
            get => _selectedFileModels;
            set
            {
                _selectedFileModels = value;
                SaveToDirCommand.RaiseCanExecuteChanged();
                DeleteCommand.RaiseCanExecuteChanged();
            }
        }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand SaveToDirCommand { get; }
        public RelayCommand AddFileToDbCommand { get; }
        public Window ParentWindow;
        public FilesPageViewModel()
        {
            FileModels = new ObservableCollection<FileModel>(FileModel.GetFileModelsFromDatabase());
            _saveFileDialog = new SaveFileDialog();
            FileModels.CollectionChanged += CollectionChanged;
            SaveToDirCommand = new RelayCommand(o =>
                {
                    _saveFileDialog.InitialFileName = "Несколько файлов";
                    _saveFileDialog.DefaultExtension = "Файлы|*.???";
                    var task = _saveFileDialog.ShowAsync(ParentWindow);
                    task.Wait();
                    if (SelectedFileModels.Any() && !string.IsNullOrWhiteSpace(task.Result))
                        FileModel.SaveMultipleFileModelsToDirectory(SelectedFileModels, _saveFileDialog.Directory);
                },o => AnyModelsSelected);
            DeleteCommand = new RelayCommand(o =>
            {
                var task = MessageBox.Avalonia.MessageBoxManager
                    .GetMessageBoxStandardWindow("Удаление", "Вы уверены, что хотите выбранные файлы")
                    .ShowDialog(ParentWindow);
                task.Wait();
                if (task.Result == ButtonResult.Yes)
                    FileModel.DeleteMultipleFilesFromDatabase(_selectedFileModels, FileModels);
            },o => AnyModelsSelected);
            SelectedFileModels = null;
        }
        private bool AnyModelsSelected => SelectedFileModels != null && SelectedFileModels.Any();
        
    }
}
