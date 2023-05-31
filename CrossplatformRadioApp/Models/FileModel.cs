using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using CrossplatformRadioApp.Context;
using CrossplatformRadioApp.Entities;

namespace CrossplatformRadioApp.Models
{
    class FileModel
    {
        private string _name, _format, _creationDateString;
        public string Name => _name; 
        public string? Format => _format==""? null: _format; 
        public string NameWithFormat => string.IsNullOrWhiteSpace(Format) ? Name: Name+"."+Format;
        private int _id;
        public string CreationDate => _creationDateString;
        private long _fileSize;
        public string FileSize => _fileSize.ToString();
        public FileModel(SavedFile databaseEntry)
        {
            _id = databaseEntry.Id;
            _name = databaseEntry.FileName;
            _format = databaseEntry.Format;
            _fileSize = databaseEntry.ByteCode.LongLength;
            _creationDateString = databaseEntry.DateOfSaving.ToLocalTime().ToShortDateString() +" "+
                                  databaseEntry.DateOfSaving.ToLocalTime().ToShortTimeString();
        }
        public void SaveToDirectory(string directory, Func<bool>? actionIfFileExist=null)
        {
            string filepath = Path.Combine(directory, NameWithFormat);
            if (File.Exists(filepath))
                if(actionIfFileExist==null || actionIfFileExist.Invoke())
                    File.Delete(filepath);
                else
                    return;

            using var database = new MyDbContext();
            using var fileStream = File.Create(filepath);
            var fileBytes = database.SavedFiles.Find(_id)?.ByteCode;
            fileStream.Write(fileBytes, 0, fileBytes.Length);
            fileStream.Flush();
        }
        public bool DeleteFromDatabase(ObservableCollection<FileModel>? observableCollection = null)
        {
            bool deleted = true;
            using (var database = new MyDbContext())
            {
                var databaseEntry = database.SavedFiles.Find(_id);
                try
                {
                    database.SavedFiles.Remove(databaseEntry);
                    database.SaveChanges();
                    observableCollection?.Remove(this);
                }
                catch
                {
                    deleted = false;
                }
            }
            return deleted;
        }
        public static List<FileModel> GetFileModelsFromDatabase()
        {
            List<FileModel> fileModels;
            using (var database = new MyDbContext())
            {
                fileModels = database.SavedFiles.ToList().Select(dbEntry => new FileModel(dbEntry)).ToList();
            }
            return fileModels;
        }
        public static void SaveMultipleFileModelsToDirectory(IEnumerable<FileModel> fileModels, string directory, Func<bool>? actionIfFileExist=null)
        {
            foreach (var fileModel in fileModels)
            {
                fileModel.SaveToDirectory(directory, actionIfFileExist);
            }
        }
        public static void DeleteMultipleFilesFromDatabase(IEnumerable<FileModel> fileModels, ObservableCollection<FileModel>? observableCollection = null)
        {
            foreach(var fileModel in fileModels)
            {
                fileModel.DeleteFromDatabase(observableCollection);
            }
        }
        public static FileModel WriteFileIntoDatabase(string filepath)
        {
            var newFile = _GetFileAsDBEntry(filepath);
            using (var database = new MyDbContext())
            {
                database.SavedFiles.Add(newFile);
                try { database.SaveChanges(); }
                catch { newFile = null; }
            }
            return newFile == null ? null : new FileModel(newFile);
        }
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
            catch (Exception ex)
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
}
