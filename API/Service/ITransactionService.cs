using API.Util;

namespace API.Service;

public interface ITransactionService
{
    Task<Option<IError>> DoTransactionAsync(Func<Task<Option<IError>>> action);
    Option<IError> DoTransaction(Func<Task<Option<IError>>> action);
}