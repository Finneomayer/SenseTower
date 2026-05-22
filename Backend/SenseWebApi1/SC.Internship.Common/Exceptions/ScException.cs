using JetBrains.Annotations;

namespace SC.Internship.Common.Exceptions;

/// <summary>
/// Базовый класс для исключений связанных с логикой работы наших приложений
/// </summary>
[UsedImplicitly]
public class ScException : Exception
{
    public ScException(string? message) : base(message) { }

    public ScException(Exception? exception, string? message) : base(message, exception) { }
}