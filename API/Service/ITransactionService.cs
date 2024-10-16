using API.Domain;
using API.Util;
using Microsoft.EntityFrameworkCore;

namespace API.Service;

public interface ITransactionService
{
    Task<Option<IError>> DoTransactionAsync(List<Lockable> dependencies, Func<Task<Option<IError>>> action);
    Option<IError> DoTransaction(List<Lockable> dependencies, Func<Task<Option<IError>>> action);
}