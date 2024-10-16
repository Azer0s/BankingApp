using API.Domain;
using API.Error;
using API.Repository;
using API.Util;

namespace API.Service.Impl;

public class UserServiceImpl(IUserRepository userRepository, ITransactionService transactionService) : IUserService
{
    public async Task<Result<User>> GetUserAsync(string id)
    {
        if (string.IsNullOrEmpty(id) || !Guid.TryParse(id, out var userId))
        {
            return Result<User>.Error(new InvalidArgumentError("id"));
        }
        
        var user = await userRepository.GetUserAsync(userId);
        return user.IsSome ? Result<User>.Ok(user.Unwrap()) : Result<User>.Error(new NotFoundError());
    }

    public async Task<Result<User>> CreateAccountForUserAsync(string id)
    {
        return (await GetUserAsync(id)).MapOrError<User>(u =>
        {
            var result = transactionService.DoTransaction([u], async () =>
            {
                if (u.PersonalBalance < 100.0m)
                {
                    return Option<IError>.Some(new InsufficientBalanceError());
                }
                
                u.PersonalBalance -= 100.0m;
                var account = new Account(Guid.NewGuid()) {Balance = 100.0m};
                
                await userRepository.AddAccountToUserAsync(u, account);
                return Option<IError>.None();
            });
            
            return result.IsSome ? Result<User>.Error(result.Unwrap()) : Result<User>.Ok(u);
        });
    }

    public async Task<Result<User>> DeleteAccountForUserAsync(string id, string accountId)
    {
        return (await GetUserAsync(id)).MapOrError(u =>
        {
            var account = u.Accounts.FirstOrDefault(a => a.Id.ToString() == accountId);
            if (account == null)
            {
                return Result<User>.Error(new NotFoundError());
            }
            
            var result = transactionService.DoTransaction([u, account], async () =>
            {
                u.PersonalBalance += account.Balance;
                await userRepository.DeleteAccountForUserAsync(u, account);
                return Option<IError>.None();
            });
            
            return result.IsSome ? Result<User>.Error(result.Unwrap()) : Result<User>.Ok(u);
        });
    }
}