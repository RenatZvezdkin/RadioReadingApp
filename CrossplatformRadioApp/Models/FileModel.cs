using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CrossplatformRadioApp.Context;
using CrossplatformRadioApp.MainDatabase;

namespace CrossplatformRadioApp.Models;
/// <summary>
/// Класс, отвечающий за показательные модели файлов, создающиеся из записей базы данных
/// </summary>
class FileModel
{
    private string _name, _format, _creationDateString;
    /// <summary>
    /// Имя файла без формата
    /// </summary>
    public string Name => _name; 
    /// <summary>
    /// Формат файла, при отсутствии null
    /// </summary>
    public string? Format => _format==""? null: _format; 
    /// <summary>
    /// Имя файла вместе с форматом. Если формат равен null, то возвращает просто имя файла
    /// </summary>
    public string NameWithFormat => string.IsNullOrWhiteSpace(Format) ? Name: Name+"."+Format;
    /// <summary>
    /// Айди оригинатора файла из базы данных
    /// </summary>
    public int Id => _id;
    private int _id;
    /// <summary>
    /// Показательная дата создания файла
    /// </summary>
    public string CreationDate => _creationDateString;
    /// <summary>
    /// Создает экземпляр модели файла с большинством необходимых параметров
    /// </summary>
    /// <param name="databaseEntry">запись из базы данных для файла</param>
    public FileModel(SavedFile databaseEntry)
    {
        _id = databaseEntry.Id;
        _name = databaseEntry.FileName;
        _format = databaseEntry.Format;
        _creationDateString = databaseEntry.DateOfSaving.ToLocalTime().ToShortDateString() +" "+
                              databaseEntry.DateOfSaving.ToLocalTime().ToShortTimeString();
    }
    /// <summary>
    /// Загружает файл из базы данных и переносит его по выбранному пути
    /// </summary>
    /// <param name="directory">путь без имени файла</param>
    /// <param name="actionIfFileExist">условие, которое, при существовании файла с таким же именем в том же пути, должно быть вполнено, чтобы заменить файл.
    /// Если равен null, то всегда заменяет прежний файл</param>
    public void SaveToDirectory(string directory, Func<string, bool>? actionIfFileExist=null)
    {
        string filepath = Path.Combine(directory, NameWithFormat);
        if (File.Exists(filepath))
            if(actionIfFileExist==null || actionIfFileExist.Invoke(filepath))
                File.Delete(filepath);
            else
                return;

        using var database = new MyDbContext();
        using var fileStream = File.Create(filepath);
        var fileBytes = database.SavedFiles.Find(_id)?.ByteCode;
        fileStream.Write(fileBytes, 0, fileBytes.Length);
        fileStream.Flush();
    }
    /// <summary>
    /// Удаляет оригинатор модели из базы данных. Если же удаление не произошло, то файл остается в коллекциях
    /// </summary>
    /// <param name="observableCollections">коллекции для удаления, в которых содержится данная модель</param>
    /// <returns>true, если удаление произошло успешно, в ином случае - false</returns>
    public bool DeleteFromDatabase(params ObservableCollection<FileModel>[] observableCollections)
    {
        bool deleted = true;
        using (var database = new MyDbContext())
        {
            var databaseEntry = database.SavedFiles.Find(_id);
            try
            {
                database.SavedFiles.Remove(databaseEntry);
                database.SaveChanges();
                foreach (var collection in observableCollections)
                    collection.Remove(this);
            }
            catch
            {
                deleted = false;
            }
        }
        return deleted;
    }
    /// <summary>
    /// Получает все записи файлов из базы данных в виде моделей
    /// </summary>
    /// <returns>коллекция моделей файлов из базы данных</returns>
    public static List<FileModel> GetFileModelsFromDatabase()
    {
        using var database = new MyDbContext();
        return database.SavedFiles.ToList().Select(dbEntry => new FileModel(dbEntry)).ToList();
    }
    /// <summary>
    /// Загружает несколько моделей файлов в директорию из базы данных, подробнее в <see cref="SaveToDirectory"/>
    /// </summary>
    /// <param name="fileModels"></param>
    /// <param name="directory"></param>
    /// <param name="actionIfFileExist">условие для каждого файла</param>
    public static void SaveMultipleFileModelsToDirectory(IEnumerable<FileModel> fileModels, string directory, Func<string, bool>? actionIfFileExist=null)
    {
        foreach (var fileModel in fileModels)
        {
            fileModel.SaveToDirectory(directory, actionIfFileExist);
        }
    }
    /// <summary>
    /// Удаляет несколько моделей файлов из базы данных, подробнее в <see cref="DeleteFromDatabase"/>
    /// </summary>
    /// <param name="fileModels">модели для удаления</param>
    /// <param name="observableCollections">коллекции, в которых модели должны быть удалены</param>
    public static void DeleteMultipleFilesFromDatabase(IEnumerable<FileModel> fileModels, params ObservableCollection<FileModel>[] observableCollections)
    {
        foreach(var fileModel in fileModels)
        {
            fileModel.DeleteFromDatabase(observableCollections);
        }
    }
    /// <summary>
    /// Записывает несколько файлов по выбранным путям в базу данных
    /// </summary>
    /// <param name="filepaths">пути к файлам</param>
    /// <returns>модели записанных файлов</returns>
    public static List<FileModel> WriteMultipleFilesIntoDatabase(IEnumerable<string> filepaths)
    {
        List<FileModel> result;
        using var database = new MyDbContext();
        result = filepaths.Select(filepath =>
        {
            var newFile = _GetFileAsDBEntry(filepath);
            database.SavedFiles.Add(newFile);
            return new FileModel(newFile);
        }).ToList();

        try
        {
            database.SaveChanges();
        }
        catch (Exception ignored)
        {
            result = null;
        }
        return result;
    }
    private static SavedFile _GetFileAsDBEntry(string filepath)
    {
        var newFile = new SavedFile
        {
            FileName = Path.GetFileNameWithoutExtension(filepath),
            Format = Path.GetExtension(filepath),
            ByteCode = File.ReadAllBytes(filepath),
            DateOfSaving = DateTime.UtcNow
        };
        if (newFile.Format.StartsWith('.'))
            newFile.Format = newFile.Format.Substring(1);
        return newFile;
    }
}
