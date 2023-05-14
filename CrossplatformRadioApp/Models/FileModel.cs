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
        private string _name;
        public string Name { get => _name; }
        private string _format;
        public string Format { get => _format; }
        private int _id;
        public FileModel(Savedfile databaseEntry)
        {
            _id = databaseEntry.Id;
            _name = databaseEntry.FileName;
            _format = databaseEntry.Format.Substring(1);
        }
        public void SaveToDirectory(string directory, Func<bool>? actionIfFileExist=null)
        {
            string filepath = Path.Combine(directory, Name+"."+Format);
            if (File.Exists(filepath))
                if(actionIfFileExist==null || actionIfFileExist.Invoke())
                    File.Delete(filepath);
                else
                    return;

            using var database = new MyDbContext();
            using var fileStream = File.Create(filepath);
            var fileBytes = database.Savedfiles.Find(_id)?.ByteCode;
            fileStream.Write(fileBytes, 0, fileBytes.Length);
            fileStream.Flush();
        }
        public bool DeleteFromDatabase(ObservableCollection<FileModel>? observableCollection = null)
        {
            bool deleted = true;
            using (var database = new MyDbContext())
            {
                var databaseEntry = database.Savedfiles.Find(_id);
                try
                {
                    database.Savedfiles.Remove(databaseEntry);
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
                fileModels = database.Savedfiles.ToList().Select(dbEntry => new FileModel(dbEntry)).ToList();
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
            var newFile = new Savedfile
            {
                FileName = Path.GetFileNameWithoutExtension(filepath),
                Format = Path.GetExtension(filepath),
                ByteCode = File.ReadAllBytes(filepath),
                DateOfSaving = DateTime.UtcNow
            };
            using (var database = new MyDbContext())
            {
                database.Savedfiles.Add(newFile);
                try { database.SaveChanges(); }
                catch { newFile = null; }
            }
            return newFile == null ? null : new FileModel(newFile);
        }
        public static List<FileModel> WriteMultipleFilesIntoDatabase(IEnumerable<string> filepaths)
        {
            List<FileModel> result;
            using (var database = new MyDbContext())
            {
                result = filepaths.Select(filepath =>
                {
                    var newFile = new Savedfile
                    {
                        FileName = Path.GetFileNameWithoutExtension(filepath),
                        Format = Path.GetExtension(filepath),
                        ByteCode = File.ReadAllBytes(filepath),
                        DateOfSaving = DateTime.UtcNow
                    };
                    database.Savedfiles.Add(newFile);
                    return new FileModel(newFile);
                }).ToList();

                try { database.SaveChanges(); }
                catch { result = null; }
            }
            return result;
        }
    }
}
