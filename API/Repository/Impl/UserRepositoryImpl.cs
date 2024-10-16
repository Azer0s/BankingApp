using API.Domain;
using API.Error;
using API.Util;
using Microsoft.EntityFrameworkCore;

namespace API.Repository.Impl;

public class UserRepositoryImpl(BankingContext context, IAccountRepository accountRepository) : IUserRepository
{
    public async Task<Option<User>> GetUserAsync(Guid id)
    {
        var userQuery = from u in context.Users
                   where u.Id == id
                   select u;
        var user = await userQuery
            .Include(u => u.Accounts)
            .FirstOrDefaultAsync();
        
        return Option<User>.FromNullable(user);
    }

    public async Task AddAccountToUserAsync(User user, Account account)
    {
        account.UserId = user.Id;
        user.Accounts.Add(account);
        await accountRepository.AddAccountAsync(account);
    }

    public async Task<Option<User>> DeleteAccountForUserAsync(User user, Account account)
    {
        user.Accounts.Remove(account);
        await accountRepository.DeleteAccountAsync(account);
        return Option<User>.Some(user);
    }
}