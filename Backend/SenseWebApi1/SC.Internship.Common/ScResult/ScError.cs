using JetBrains.Annotations;

namespace SC.Internship.Common.ScResult;

/// <summary>
/// Класс ошибки, возвращаемой из методов контроллеров
/// </summary>
[UsedImplicitly]
public class ScError
{
    /// <summary>
    /// Интегральное сообщение об ошибке
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public string? Message { get; set; }
    /// <summary>
    /// Сообщения об ошибках с привязкой к параметрам входной модели Acton
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public Dictionary<string, List<string>>? ModelState { get; set; }
    
}