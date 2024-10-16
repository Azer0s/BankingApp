using API.Domain;
using API.Error;
using API.Service;
using API.Util;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/account/{id}")]
[ApiController]
public class AccountController(ILogger<UserController> logger, IAccountService accountService)
    : ControllerBase
{
    private IActionResult MapAccountResult(Result<Account> result) => result
        .Map<IActionResult>(Ok)
        .OrElse(f => f switch
        {
            NotFoundError => NotFound(),
            _ => StatusCode(500, f.Message)
        });
    
    [HttpGet(Name = "GetAccount")]
    public async Task<IActionResult> Get([FromRoute] string id)
    {
        logger.LogDebug($"Getting account with id {id}");
        return MapAccountResult(await accountService.GetAccountAsync(id));
    }

    [HttpPost("withdraw")]
    public async Task<IActionResult> Withdraw([FromRoute] string id, [FromBody] decimal amount)
    {
        logger.LogDebug($"Withdrawing {amount} from account with id {id}");
        return MapAccountResult(await accountService.Withdraw(id, amount));
    }
    
    [HttpPost("deposit")]
    public async Task<IActionResult> Deposit([FromRoute] string id, [FromBody] decimal amount)
    {
        logger.LogDebug($"Depositing {amount} to account with id {id}");
        return MapAccountResult(await accountService.Deposit(id, amount));
    }
}