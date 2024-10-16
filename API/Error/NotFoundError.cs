using API.Util;

namespace API.Error;

public record NotFoundError : IError
{
    public string Message => "Not found";
}