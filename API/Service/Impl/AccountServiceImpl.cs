using API.Domain;
using API.Error;
using API.Repository;
using API.Util;

namespace API.Service.Impl;

public class AccountServiceImpl(IAccountRepository accountRepository, IUserRepository userRepository, ITransactionService transactionService) : IAccountService
{
    public async Task<Result<Account>> GetAccountAsync(string id)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var accountId))
        {
            return Result<Account>.Error(new InvalidArgumentError("id"));
        }
        
        var account = await accountRepository.GetAccountAsync(accountId);
        return account.IsSome ? Result<Account>.Ok(account.Unwrap()) : Result<Account>.Error(new NotFoundError());
    }

    public async Task<Result<Account>> Withdraw(string id, decimal amount)
    {
        return (await GetAccountAsync(id)).MapOrError(a =>
        {
            var user = userRepository.GetUserAsync(a.UserId).Result.Unwrap();
            var result = transactionService.DoTransaction([a, user], async () =>
            {
                switch (a.Balance - amount)
                {
                    case < 0:
                        return Option<IError>.Some(new InsufficientBalanceError());
                    case < 100:
                        return Option<IError>.Some(new InvalidTransactionError("withdrawal amount cannot reduce balance below 100$"));
                }

                var totalBalance = user.Accounts.ToList().Aggregate(0m, (acc, account) => account.Balance + acc);

                if (amount > 0.9m * totalBalance)
                {
                    return Option<IError>.Some(new InvalidTransactionError("withdrawal amount cannot exceed 90% of total balance"));
                }
                
                a.Balance -= amount;
                await accountRepository.UpdateAccountAsync(a);
                return Option<IError>.None();
            });
            
            return result.IsSome ? Result<Account>.Error(result.Unwrap()) : Result<Account>.Ok(a);
        });
    }

    public async Task<Result<Account>> Deposit(string id, decimal amount)
    {
        if (amount > 10_000m)
        {
            return Result<Account>.Error(new InvalidTransactionError("deposit amount cannot exceed 10,000$"));
        }
        
        return (await GetAccountAsync(id)).MapOrError(a =>
        {
            var result = transactionService.DoTransaction([a], async () =>
            {
                a.Balance += amount;
                await accountRepository.UpdateAccountAsync(a);
                return Option<IError>.None();
            });
            
            return result.IsSome ? Result<Account>.Error(result.Unwrap()) : Result<Account>.Ok(a);
        });
    }
}