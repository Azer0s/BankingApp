using System.Data;
using API.Domain;
using API.Repository;
using API.Util;
using API.Util.Stereotype;
using Microsoft.EntityFrameworkCore;

namespace API.Service.Impl;

public class TransactionServiceImpl(BankingContext context): ITransactionService
{
    public async Task<Option<IError>> DoTransactionAsync(List<DomainObject> dependencies,
        Func<Task<Option<IError>>> action)
    {
        await using var transaction = await context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        // we need to disable this warning because we need to use raw SQL for our pessimistic locking shenanigans
#pragma warning disable EF1002
        foreach (var dependency in dependencies)
        {
            var (lockOnTable, idField, lockOnId) = dependency.AcquireLock();
            await context.Database.ExecuteSqlRawAsync($"SELECT * FROM \"{lockOnTable}\" WHERE \"{idField}\" = {lockOnId} FOR UPDATE NOWAIT;");
        }
#pragma warning restore EF1002
        
        var res = await action();
        if (res.IsSome)
        {
            await transaction.RollbackAsync();
            return res;
        }

        await transaction.CommitAsync();
        return Option<IError>.None();
    }
    
    public Option<IError> DoTransaction(List<DomainObject> dependencies, Func<Task<Option<IError>>> action)
    {
        return Task.Run(() => DoTransactionAsync(dependencies, action)).Result;
    }
}