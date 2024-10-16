using API.Domain;
using API.Util;

namespace API.Service;

public interface IUserService
{
    Task<Result<User>> GetUserAsync(string id);
    Task<Result<User>> CreateAccountForUserAsync(string id);
    Task<Result<User>> DeleteAccountForUserAsync(string id, string accountId);
}