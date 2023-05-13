using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Platform.Storage;
using CrossplatformRadioApp.Models;
using CrossplatformRadioApp.Views;
using MessageBox.Avalonia.Enums;
using ReactiveUI;

namespace CrossplatformRadioApp.ViewModels
{
    class FilesPageViewModel : ViewModelBase
    {
        private FilePickerSaveOptions _saveFileDialog;
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
            _saveFileDialog = new FilePickerSaveOptions();
            SaveToDirCommand = new RelayCommand(o =>
            {
                _saveFileDialog.DefaultExtension = "???";
                _saveFileDialog.SuggestedFileName = "Несколько файлов";
                using (var window = Manager.Instance.MainWindow.StorageProvider.SaveFilePickerAsync(_saveFileDialog))
                {
                    window.Wait();
                    var localPath = window.Result?.TryGetLocalPath();
                    if (SelectedFileModels.Any() && !string.IsNullOrWhiteSpace(localPath))
                        FileModel.SaveMultipleFileModelsToDirectory(SelectedFileModels, localPath);
                }
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
        
    }
}
