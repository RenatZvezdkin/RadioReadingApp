using System.IO;
using System.Reflection;

namespace CrossplatformRadioApp;

public class Manager
{
    private string _getExecutingPath { get => Path.GetFullPath(Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"..\..\..\")); }
    public string ConnectionString
    {
        get
        {
            return File.ReadAllText(_getExecutingPath+"connectionstring.txt");
        }
    }

    private static Manager instance = new Manager();
    public static Manager Instance => instance;
    
}