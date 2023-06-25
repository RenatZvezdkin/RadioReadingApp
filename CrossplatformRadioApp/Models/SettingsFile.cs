using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CrossplatformRadioApp;

public class SettingsFile: ISettingsFile
{
    private readonly List<string> _properties;
    public string FilePath { get; }
    public List<string> Properties 
        => new(_properties);
    public SettingsFile(string path)
    {
        
        FilePath = Path.Combine(path, "settings.settings");
        if (File.Exists(FilePath))
            _properties = File.ReadAllLines(FilePath).Select(line =>
                Encoding.ASCII.GetString(Convert.FromBase64String(line)).Split(':')[0]
            ).ToList();
        else
        {
            File.WriteAllText(FilePath, "");
            _properties = new List<string>();
        }
    }
    /// <summary>
    /// Проверяет, имеется ли текущее свойство в файле настроек, и может ли оно быть добавлено
    /// </summary>
    /// <param name="property">свойство для проверки наличия в файле и правильности написания</param>
    /// <param name="value">его значение</param>
    /// <returns>true, если оба значения принадлежат алфавиту ASCII, свойство состоит только из букв и не существует на текущий момент</returns>
    public bool CanBeAdded(string property, string value)
    {
        return !_properties.Contains(property.Trim()) && property.All(char.IsAscii) && property.All(char.IsLetter) && value.All(char.IsAscii);
    }
    /// <summary>
    /// Добавляет свойство в файл настроек со значением, если оно еще не существует и подходит по условиям (Подробнее в <see cref="CanBeAdded"/>)
    /// </summary>
    /// <param name="property">свойство для добавления в файл</param>
    /// <param name="value">значение свойства</param>
    public void AddProperty(string property, string value)
    {
        if (!CanBeAdded(property, value))
            return;
        var newLine = property+":"+value;
        _properties.Add(property);
        var lines = File.ReadAllLines(FilePath).ToList();
        lines.Add(Convert.ToBase64String(Encoding.ASCII.GetBytes(newLine)));
        File.Delete(FilePath);
        File.WriteAllLines(FilePath, lines);
    }

    public string? GetValueFromProperty(string property)
    {
        property = property.Trim();
        return (from line in File.ReadAllLines(FilePath) select Encoding.ASCII.GetString(Convert.FromBase64String(line)).Split(":", 2) into parts where property == parts[0].Trim() select parts[1].Trim()).FirstOrDefault();
    }
    public void Recreate()
    {
        if (File.Exists(FilePath))
            File.Delete(FilePath);
        File.WriteAllText(FilePath, "");
    }
}