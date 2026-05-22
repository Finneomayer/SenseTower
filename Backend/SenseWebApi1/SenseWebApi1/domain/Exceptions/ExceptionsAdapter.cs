
namespace SenseWebApi1.domain.Exceptions;

public class ExceptionsAdapter:Exception
{
    // ReSharper disable once AutoPropertyCanBeMadeGetOnly.Global
    public Dictionary<string,List<string>> Exceptions { get; set; }
    public ExceptionsAdapter(Dictionary<string,List<string>> errors)
    {
        Exceptions=errors;
    }

}