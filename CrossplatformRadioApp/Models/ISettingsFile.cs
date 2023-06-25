using System.Collections.Generic;

namespace CrossplatformRadioApp;

/// <summary>
/// Интерфейс, отражающий все необходимое для файла настроек, с целью упрощения изменения методов шифрования в будущем
/// </summary>
public interface ISettingsFile
{
    /// <summary>
    /// Путь, в котором находится файл настроек
    /// </summary>
    string FilePath { get; }
    /// <summary>
    /// Все существующие свойства файла настроек
    /// </summary>
    List<string> Properties { get; }
    /// <summary>
    /// Проверяет, имеется ли текущее свойство в файле настроек, и может ли оно быть добавлено
    /// </summary>
    /// <param name="property">свойство для проверки наличия в файле и правильности написания</param>
    /// <param name="value">его значение</param>
    /// <returns>true, если свойство не занято и может быть добавлено, иначе false</returns>
    bool CanBeAdded(string property, string value);
    /// <summary>
    /// Добавляет свойство в файл настроек со значением
    /// </summary>
    /// <param name="property">свойство для добавления в файл</param>
    /// <param name="value">значение свойства</param>
    void AddProperty(string property, string value);
    /// <summary>
    /// Получает значение свойства из файла настроек
    /// </summary>
    /// <param name="property">название свойства, чье значение должно быть получено/param>
    /// <returns></returns>
    string? GetValueFromProperty(string property);
    /// <summary>
    /// Пересоздает файл настроек (полезно в случаях, когда база данных утратила актуальность)
    /// </summary>
    void Recreate();
}