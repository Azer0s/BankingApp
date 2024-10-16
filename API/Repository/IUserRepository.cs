using API.Domain;
using API.Util;
using IError = API.Util.IError;

namespace API.Repository;

public interface IUserRepository
{
    Task<Option<User>> GetUserAsync(Guid id);
    Task AddAccountToUserAsync(User user, Account account);
    Task<Option<User>> DeleteAccountForUserAsync(User user, Account account);
}