using API.Domain;
using API.Repository;
using API.Repository.Impl;
using API.Service;
using API.Service.Impl;
using API.Util;
using API.Util.Stereotype;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Moq;

namespace API.Test;

[TestFixture]
public class TestAccountCreate
{
    private BankingContext? _context;
    private Guid? _userId;
    private IUserService? _userService;
    private IAccountService? _accountService;
    
    [SetUp]
    public async Task Setup()
    {
        var options = new DbContextOptionsBuilder<BankingContext>()
            .UseInMemoryDatabase("Test")
            .Options;

        _context = new BankingContext(options);
        var accountRepository = new AccountRepositoryImpl(_context);
        var userRepository = new UserRepositoryImpl(_context, accountRepository);
        
        var transactionServiceMock = new Mock<ITransactionService>();
        transactionServiceMock
            .Setup(t => t.DoTransaction(It.IsAny<List<DomainObject>>(), It.IsAny<Func<Task<Option<IError>>>>()))
            .Returns(new Func<List<DomainObject>, Func<Task<Option<IError>>>, Option<IError>>((_, f) => Task.Run(async () => await f()).Result));
        transactionServiceMock.Setup(t => t.DoTransactionAsync(It.IsAny<List<DomainObject>>(), It.IsAny<Func<Task<Option<IError>>>>()))
            .Returns(new Func<List<DomainObject>, Func<Task<Option<IError>>>, Task<Option<IError>> >((_, f) => f()));
        
        _userService = new UserServiceImpl(userRepository, transactionServiceMock.Object);
        _accountService = new AccountServiceImpl(accountRepository, userRepository, transactionServiceMock.Object);

        var user = new User(Guid.NewGuid())
        {
            PersonalBalance = 1000
        };
        
        _userId = user.Id;

        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }
    
    [TearDown]
    public async Task TearDown()
    {
        _context!.Accounts.RemoveRange(_context.Accounts);
        _context.Users.RemoveRange(_context.Users);
        await _context.SaveChangesAsync();
    }
    
    [Test]
    public async Task TestTakePersonalBalanceForAccountCreation()
    {
        var user = (await _userService!.GetUserAsync(_userId!.ToString()!))
            .OrElseThrow();
        var previousBalance = user.PersonalBalance;
        
        var userFromDb = (await _userService.CreateAccountForUserAsync(user.Id.ToString()))
            .OrElseThrow();
        
        ClassicAssert.AreEqual(userFromDb.PersonalBalance, previousBalance - 100);
        ClassicAssert.AreEqual(100, userFromDb.Accounts.First().Balance);
    }
    
    [Test]
    public async Task TestTakePersonalBalanceForAccountCreationError()
    {
        var user = (await _userService!.GetUserAsync(_userId!.ToString()!))
            .OrElseThrow();
        user.PersonalBalance = 0;
        await _context!.SaveChangesAsync();

        var userFromDb = (await _userService.CreateAccountForUserAsync(user.Id.ToString()));
        ClassicAssert.IsTrue(userFromDb.IsError);
    }
    
    [Test]
    public async Task TestCannotDepositOver10KIntoAccount()
    {
        var user = (await _userService!.CreateAccountForUserAsync(_userId.ToString()!))
            .OrElseThrow();
        var account = user.Accounts.First();
        
        var result = await _accountService!.Deposit(account.Id.ToString(), 10001);
        ClassicAssert.IsTrue(result.IsError);
    }
    
    [Test]
    public async Task TestCanDepositIntoAccount()
    {
        var user = (await _userService!.CreateAccountForUserAsync(_userId.ToString()!))
            .OrElseThrow();
        var account = user.Accounts.First();
        
        var result = await _accountService!.Deposit(account.Id.ToString(), 900);
        ClassicAssert.IsTrue(result.IsOk);
        ClassicAssert.AreEqual(1000, result.OrElseThrow().Balance);
    }
    
    [Test]
    public async Task TestCanWithdrawFromAccount()
    {
        var user = (await _userService!.CreateAccountForUserAsync(_userId.ToString()!))
            .OrElseThrow();
        var account = user.Accounts.First();
        
        var result = await _accountService!.Deposit(account.Id.ToString(), 900);
        ClassicAssert.IsTrue(result.IsOk);
        ClassicAssert.AreEqual(1000, result.OrElseThrow().Balance);
        
        result = await _accountService!.Withdraw(account.Id.ToString(), 900);
        ClassicAssert.IsTrue(result.IsOk);
        ClassicAssert.AreEqual(100, result.OrElseThrow().Balance);
    }
    
    [Test]
    public async Task TestCannotWithdrawSoAccountBalanceIsLessThan100()
    {
        var user = (await _userService!.CreateAccountForUserAsync(_userId.ToString()!))
            .OrElseThrow();
        var account = user.Accounts.First();
        
        var result = await _accountService!.Withdraw(account.Id.ToString(), 10);
        ClassicAssert.IsTrue(result.IsError);
    }
    
    [Test]
    public async Task TestCannotWithdrawMoreThan90PercentTotalUserBalance()
    {
        var user = (await _userService!.CreateAccountForUserAsync(_userId.ToString()!))
            .OrElseThrow();
        var account = user.Accounts.First();
        
        var result = await _accountService!.Deposit(account.Id.ToString(), 1000);
        ClassicAssert.IsTrue(result.IsOk);
        
        // 990 is 90% of 1100 so 991 should be an error
        result = await _accountService!.Withdraw(account.Id.ToString(), 991);
        ClassicAssert.IsTrue(result.IsError);
    }
    
    [Test]
    public async Task TestCannotWithdrawMoreThan90PercentTotalUserBalance2()
    {
        var user = (await _userService!.CreateAccountForUserAsync(_userId.ToString()!))
            .OrElseThrow();
        var account = user.Accounts.First();
        
        var result = await _accountService!.Deposit(account.Id.ToString(), 1500);
        ClassicAssert.IsTrue(result.IsOk);
        var withdrawAccount = result.OrElseThrow().Id;
        
        // create another account
        user = (await _userService.CreateAccountForUserAsync(_userId.ToString()!))
            .OrElseThrow();
        account = user.Accounts.First(a => a.Balance == 100m);
        
        result = await _accountService!.Deposit(account.Id.ToString(), 100);
        ClassicAssert.IsTrue(result.IsOk);
        
        // total balance is 1600 + 200 = 1800
        // 90% of 1800 is 1620 so 1621 should be an error
        result = await _accountService!.Withdraw(withdrawAccount.ToString(), 1621);
        ClassicAssert.IsTrue(result.IsError);
    }
}