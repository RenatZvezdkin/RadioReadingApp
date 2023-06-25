using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace CrossplatformRadioApp;

public class SettingsFile
{
    private string _pathToFile;
    private List<string> _properties;
    public List<string> GetProperties 
        => new(_properties);
    public SettingsFile(string path)
    {
        
        _pathToFile = Path.Combine(path, "settings.settings");
        if (File.Exists(_pathToFile))
            _properties = File.ReadAllLines(_pathToFile).Select(line =>
                Encoding.ASCII.GetString(Convert.FromBase64String(line)).Split(':')[0]
            ).ToList();
        else
        {
            File.WriteAllText(_pathToFile, "");
            _properties = new List<string>();
        }
    }
    
    public bool CanBeAdded(string property, string value)
    {
        return !_properties.Contains(property.Trim()) && property.All(char.IsAscii) && property.All(char.IsLetter) && value.All(char.IsAscii);
    }
    
    public void AddProperty(string property, string value)
    {
        if (!CanBeAdded(property, value))
            return;
        var newLine = property+":"+value;
        var lines = File.ReadAllLines(_pathToFile).ToList();
        lines.Add(Convert.ToBase64String(Encoding.ASCII.GetBytes(newLine)));
        File.Delete(_pathToFile);
        File.WriteAllLines(_pathToFile, lines);
    }

    public string? GetValueFromProperty(string property)
    {
        property = property.Trim();
        return (from line in File.ReadAllLines(_pathToFile) select Encoding.ASCII.GetString(Convert.FromBase64String(line)).Split(":", 2) into parts where property == parts[0].Trim() select parts[1].Trim()).FirstOrDefault();
    }

    public void Recreate()
    {
        if (File.Exists(_pathToFile))
            File.Delete(_pathToFile);
        File.WriteAllText(_pathToFile, "");
    }
}