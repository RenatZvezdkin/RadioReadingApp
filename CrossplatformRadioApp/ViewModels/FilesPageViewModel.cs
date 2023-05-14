using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Controls.Selection;
using Avalonia.Platform.Storage;
using CrossplatformRadioApp.Models;
using DynamicData;
using MessageBox.Avalonia.Enums;

namespace CrossplatformRadioApp.ViewModels
{
    class FilesPageViewModel : ViewModelBase, INotifyCollectionChanged
    {
        public FilesPageViewModel()
        {
            SelectModel = new SelectionModel<FileModel>
            {
                SingleSelect = false,
            };
            SelectModel.SelectionChanged += SelectedItemsChangeEvent;
            FileModels = new ObservableCollection<FileModel>(FileModel.GetFileModelsFromDatabase());
            _saveFileDialog = new FilePickerSaveOptions
            {
                SuggestedFileName = "Выбранный(ые) файл(ы)"
            };
            _openFileDealog = new FilePickerOpenOptions
            {
                AllowMultiple = true
            };
            FileModels.CollectionChanged += CollectionChanged;
            AddFileToDbCommand = new RelayCommand(async o =>
            {
                var result = await Manager.Instance.MainWindow.StorageProvider.
                    OpenFilePickerAsync(_openFileDealog);
                var addedFileModels = FileModel.WriteMultipleFilesIntoDatabase(
                    result.
                        Select(storageFile => storageFile.TryGetLocalPath()).
                        Where(path => !string.IsNullOrWhiteSpace(path)));
                FileModels.AddRange(addedFileModels);
            });
            SaveToDirCommand = new RelayCommand(async o =>
            {
                _saveFileDialog.SuggestedStartLocation = await TopLevel.
                    GetTopLevel(Manager.Instance.MainWindow).
                    StorageProvider.
                    TryGetWellKnownFolderAsync(WellKnownFolder.Desktop);
                var localPath = (await Manager.Instance.MainWindow.StorageProvider.
                        SaveFilePickerAsync(_saveFileDialog))?.
                    TryGetLocalPath();
                if (AnyModelsSelected && !string.IsNullOrWhiteSpace(localPath))
                    FileModel.SaveMultipleFileModelsToDirectory(SelectedFileModels, Path.GetDirectoryName(localPath));
            },o => AnyModelsSelected);
            DeleteCommand = new RelayCommand(async o =>
            {
                using var task = MessageBox.Avalonia.MessageBoxManager.
                    GetMessageBoxStandardWindow("Удаление", "Вы уверены, что хотите удалить выбранные файлы?", ButtonEnum.YesNo).
                    ShowDialog(Manager.Instance.MainWindow);
                if (await task == ButtonResult.Yes)
                    FileModel.DeleteMultipleFilesFromDatabase(SelectedFileModels, FileModels);
            },o => AnyModelsSelected);
        }
        private FilePickerSaveOptions _saveFileDialog;
        private FilePickerOpenOptions _openFileDealog;
        public ObservableCollection<FileModel> FileModels { get; }
        public RelayCommand DeleteCommand { get; }
        public RelayCommand SaveToDirCommand { get; }
        public RelayCommand AddFileToDbCommand { get; }
        public event NotifyCollectionChangedEventHandler? CollectionChanged;
        public SelectionModel<FileModel> SelectModel { get; }
        private bool AnyModelsSelected => SelectModel.SelectedItems.Any();
        public List<FileModel> SelectedFileModels => SelectModel.SelectedItems.ToList();

        void SelectedItemsChangeEvent(object? sender, SelectionModelSelectionChangedEventArgs e)
        {
            SaveToDirCommand.RaiseCanExecuteChanged();
            DeleteCommand.RaiseCanExecuteChanged();
        }
    }
}
