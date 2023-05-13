using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Avalonia.Platform.Storage;
using CrossplatformRadioApp.Models;
using MessageBox.Avalonia.Enums;
using ReactiveUI;

namespace CrossplatformRadioApp.ViewModels
{
    class FilesPageViewModel : ViewModelBase, INotifyCollectionChanged
    {
        private FilePickerSaveOptions _saveFileDialog;
        private FilePickerOpenOptions _openFileDealog;
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
        public FilesPageViewModel()
        {
            FileModels = new ObservableCollection<FileModel>(FileModel.GetFileModelsFromDatabase());
            _saveFileDialog = new FilePickerSaveOptions
            {
                DefaultExtension = "???",
                SuggestedFileName = "Несколько файлов"
            };
            _openFileDealog = new FilePickerOpenOptions
            {
                AllowMultiple = true
            };
            FileModels.CollectionChanged += CollectionChanged;
            AddFileToDbCommand = new RelayCommand(o =>
            {
                using var window = Manager.Instance.MainWindow.StorageProvider.OpenFilePickerAsync(_openFileDealog);
                window.Wait();
                FileModel.WriteMultipleFilesIntoDatabase(
                    window.Result.
                        Select(storageFile => storageFile?.TryGetLocalPath()).
                        Where(path => !string.IsNullOrWhiteSpace(path)));
            });
            SaveToDirCommand = new RelayCommand(o =>
            {
                using var window = Manager.Instance.MainWindow.StorageProvider.SaveFilePickerAsync(_saveFileDialog);
                window.Wait();
                var localPath = window.Result?.TryGetLocalPath();
                if (SelectedFileModels.Any() && !string.IsNullOrWhiteSpace(localPath))
                    FileModel.SaveMultipleFileModelsToDirectory(SelectedFileModels, localPath);
            },o => AnyModelsSelected);
            DeleteCommand = new RelayCommand(o =>
            {
                var task = MessageBox.Avalonia.MessageBoxManager
                    .GetMessageBoxStandardWindow("Удаление", "Вы уверены, что хотите выбранные файлы")
                    .ShowDialog(Manager.Instance.MainWindow);
                task.Wait();
                if (task.Result == ButtonResult.Yes)
                    FileModel.DeleteMultipleFilesFromDatabase(_selectedFileModels, FileModels);
            },o => AnyModelsSelected);
            SelectedFileModels = null;
        }
        private bool AnyModelsSelected => SelectedFileModels != null && SelectedFileModels.Any();

        public event NotifyCollectionChangedEventHandler? CollectionChanged;
    }
}
