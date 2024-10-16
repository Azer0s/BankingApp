using API.Domain;
using API.Util;

namespace API.Service;

public interface IAccountService
{
    Task<Result<Account>> GetAccountAsync(string id);
    Task<Result<Account>> Withdraw(string id, decimal amount);
    Task<Result<Account>> Deposit(string id, decimal amount);
}