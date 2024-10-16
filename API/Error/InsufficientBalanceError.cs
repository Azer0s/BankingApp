using API.Util;

namespace API.Error;

public record InsufficientBalanceError : IError
{
    public string Message => "Insufficient balance";
}