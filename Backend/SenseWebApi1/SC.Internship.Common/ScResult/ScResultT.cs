using JetBrains.Annotations;

namespace SC.Internship.Common.ScResult;

/// <summary>
/// Класс для результата возвращаемого из контроллеров с не пустым возвращаемым значениями типа T
/// </summary>
/// <typeparam name="T">Тип возвращаемого значения</typeparam>
[UsedImplicitly]
public class ScResult<T> : ScResult
{
    public ScResult()
    {
    }

    public ScResult(ScError error) : base(error)
    {
    }

    public ScResult(T result) : base(null)
    {
        Result = result;
    }

    // ReSharper disable once PropertyCanBeMadeInitOnly.Global
    public T? Result { get; set; }
}