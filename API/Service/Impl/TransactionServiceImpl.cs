using API.Repository;
using API.Util;

namespace API.Service.Impl;

public class TransactionServiceImpl(BankingContext context): ITransactionService
{
    public async Task<Option<IError>> DoTransactionAsync(Func<Task<Option<IError>>> action)
    {
        await using var transaction = await context.Database.BeginTransactionAsync();
        var res = await action();
        if (res.IsSome)
        {
            await transaction.RollbackAsync();
            return res;
        }

        await transaction.CommitAsync();
        return Option<IError>.None();
    }
    
    public Option<IError> DoTransaction(Func<Task<Option<IError>>> action)
    {
        return Task.Run(() => DoTransactionAsync(action)).Result;
    }
}