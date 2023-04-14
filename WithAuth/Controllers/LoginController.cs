using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WithAuth.Auth;

namespace WithAuth.Controllers;

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

            var result = _authenticateService.Login(request.username, request.password, new CancellationToken());
            return await result.ContinueWith(r => Ok(r));
            
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
            return await _authenticateService.AuthenticateFromRefresh(request.RefreshToken, new CancellationToken(false)).ContinueWith(r => Ok(r));
            
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return Unauthorized();
        }
    }
}

public class AuthRequest
{
    public string username { get; set; }
    public string password { get; set; }
}

public class RefreshRequest
{
    /// <summary>
    /// The refresh token.
    /// </summary>
    public string RefreshToken { get; set; }
}