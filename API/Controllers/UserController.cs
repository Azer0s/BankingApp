using API.Domain;
using API.Error;
using API.Service;
using API.Util;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[Route("api/users/{id}")]
[ApiController]
public class UserController(ILogger<UserController> logger, IUserService userService)
    : ControllerBase
{
    private IActionResult MapUserResult(Result<User> result) => result
        .Map<IActionResult>(Ok)
        .OrElse(f => f switch
        {
            NotFoundError => NotFound(),
            _ => StatusCode(500, f.Message)
        });
    
    [HttpGet(Name = "GetUser")]
    public async Task<IActionResult> GetUser([FromRoute] string id)
    {
        logger.LogDebug($"Getting user with id {id}");
        var user = await userService.GetUserAsync(id);
        return MapUserResult(user);
    }

    [HttpPost("accounts",Name = "CreateAccountForUser")]
    public async Task<IActionResult> CreateAccountForUser([FromRoute] string id)
    {
        logger.LogDebug($"Creating account for user with id {id}");
        var user = await userService.CreateAccountForUserAsync(id);
        return MapUserResult(user);
    }
    
    [HttpDelete("accounts/{accountId}",Name = "DeleteAccountForUser")]
    public async Task<IActionResult> DeleteAccountForUser([FromRoute] string id, [FromRoute] string accountId)
    {
        logger.LogDebug($"Deleting account {accountId} for user with id {id}");
        var user = await userService.DeleteAccountForUserAsync(id, accountId);
        return MapUserResult(user);
    }
}