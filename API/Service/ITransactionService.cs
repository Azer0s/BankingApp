using API.Domain;
using API.Util;
using API.Util.Stereotype;
using Microsoft.EntityFrameworkCore;

namespace API.Service;

public interface ITransactionService
{
    Task<Option<IError>> DoTransactionAsync(List<DomainObject> dependencies, Func<Task<Option<IError>>> action);
    Option<IError> DoTransaction(List<DomainObject> dependencies, Func<Task<Option<IError>>> action);
}