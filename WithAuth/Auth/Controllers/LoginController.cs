using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WithAuth.Auth.Models;

namespace WithAuth.Auth.Controllers;

[ApiController]
[Route("[controller]")]
public class LoginController : ControllerBase
{

    private readonly IAuthenticateService _authenticateService;

    public LoginController(IAuthenticateService authenticateService)
    {
        _authenticateService = authenticateService;
    }
    
    [AllowAnonymous]
    [HttpPost]
    // Notice: We get a custom request object from the body
    public async Task<IActionResult> Login([FromBody] AuthRequest request)
    {
        try
        {
            if (request is { Username: not null, Password: not null })
            {
                var result = _authenticateService.Login(request.Username, request.Password, new CancellationToken());
                return await result.ContinueWith(r => Ok(r));
            }
            return BadRequest();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Unauthorized();
        }
    }
    
    [AllowAnonymous]
    [HttpPost]
    [Route("refresh")]
    // Notice: We get a custom request object from the body
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        try
        {
            if (request.RefreshToken != null)
            {
                return await _authenticateService
                    .AuthenticateFromRefresh(request.RefreshToken, new CancellationToken(false))
                    .ContinueWith(r => Ok(r));
            }
            return BadRequest();

        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Unauthorized();
        }
    }
}



