using JetBrains.Annotations;

namespace SC.Internship.Common.ScResult;

/// <summary>
/// Базовый класс для результата возвращаемого из методов контроллеров
/// </summary>
[UsedImplicitly]
public class ScResult
{
    /// <summary>
    /// Информация об ошибке для случая если возникла ошибка при вызове метода
    /// </summary>
    // ReSharper disable once UnusedAutoPropertyAccessor.Global
    public ScError? Error { get; set; }

    public ScResult()
    {
    }

    public ScResult(ScError? error)
    {
        Error = error;
    }
}