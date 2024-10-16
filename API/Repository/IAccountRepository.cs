using API.Domain;
using API.Util;

namespace API.Repository;

public interface IAccountRepository
{
    Task<Option<Account>> GetAccountAsync(Guid id);
    Task AddAccountAsync(Account account);
    Task DeleteAccountAsync(Account account);
    Task UpdateAccountAsync(Account account);
}