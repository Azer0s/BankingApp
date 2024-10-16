using API.Util;

namespace API.Error;

public record InvalidTransactionError(string Message) : IError;