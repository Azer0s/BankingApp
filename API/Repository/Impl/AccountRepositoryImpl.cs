using API.Domain;
using API.Util;
using Microsoft.EntityFrameworkCore;

namespace API.Repository.Impl;

public class AccountRepositoryImpl(BankingContext context) : IAccountRepository
{
    public async Task<Option<Account>> GetAccountAsync(Guid id)
    {
        var accountQuery = from a in context.Accounts
            where a.Id == id
            select a;
        var account = await accountQuery
            .FirstOrDefaultAsync();

        return Option<Account>.FromNullable(account);
    }

    public async Task AddAccountAsync(Account account)
    {
        context.Accounts.Add(account);
        await context.SaveChangesAsync();
    }

    public Task DeleteAccountAsync(Account account)
    {
        context.Accounts.Remove(account);
        return context.SaveChangesAsync();
    }

    public Task UpdateAccountAsync(Account account)
    {
        context.Accounts.Update(account);
        return context.SaveChangesAsync();
    }
}