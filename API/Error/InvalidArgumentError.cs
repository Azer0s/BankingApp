using API.Util;

namespace API.Error;

public class InvalidArgumentError(string argument) : IError
{
    public string Message => $"Invalid argument \"{argument}\"";
}