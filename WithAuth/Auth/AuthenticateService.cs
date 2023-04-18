using System.Diagnostics;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Novell.Directory.Ldap;
using WithAuth.Auth.Token;
using WithAuth.Data;
using WithAuth.Models;
using WithAuth.Models.Exception;

namespace WithAuth.Auth;

public class AuthenticateService : IAuthenticateService
{
    private readonly IAccessTokenService _accessTokenService;
    private readonly IRefreshTokenValidator _refreshTokenValidator;
    private readonly IRefreshTokenService _refreshTokenService;
    private readonly IApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private const string SearchBase = "ou=users,dc=wimpi,dc=net";

    public AuthenticateService(IAccessTokenService accessTokenService, IRefreshTokenService refreshTokenService,
        IApplicationDbContext context, UserManager<User> userManager, IRefreshTokenValidator refreshTokenValidator)
    {
        _accessTokenService = accessTokenService;
        _refreshTokenService = refreshTokenService;
        _context = context;
        _userManager = userManager;
        _refreshTokenValidator = refreshTokenValidator;
    }

    public async Task<AuthenticateResponse> Authenticate(User user, CancellationToken cancellationToken)
    {
        var u = await _userManager.FindByNameAsync(user.UserName);
        if (u is null)
        {
            var i = await _userManager.CreateAsync(user);
            if (i.Succeeded)
                u = user;
            else
            {
                throw new Exception("Unable to create user");
            }
        }

        var refreshToken = _refreshTokenService.Generate(u);
        await _context.RefreshTokens.AddAsync(new RefreshToken(u.Id, refreshToken), cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return new AuthenticateResponse
        {
            AccessToken = _accessTokenService.Generate(u),
            RefreshToken = refreshToken
        };
    }

    public async Task<AuthenticateResponse> AuthenticateFromRefresh(string refresh, CancellationToken cancellationToken)
    {
        var isValidRefreshToken = _refreshTokenValidator.Validate(refresh);
        if (!isValidRefreshToken)
            throw InvalidRefreshTokenException.Instance;
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(x => x.Token == refresh, cancellationToken);
        if (refreshToken is null)
            throw InvalidRefreshTokenException.Instance;

        _context.RefreshTokens.Remove(refreshToken);
        await _context.SaveChangesAsync(cancellationToken);

        var user = await _userManager.FindByIdAsync(refreshToken.UserId);
        if (user is null) throw UserNotFoundException.Instance;

        return await Authenticate(user, cancellationToken);
    }

    public async Task<AuthenticateResponse> Login(string username, string password, CancellationToken cancellationToken)
    {
        var dn = $"uid={username},{SearchBase}";
        using var ldapConnection = new LdapConnection();
        // connect
        ldapConnection.Connect("127.0.0.1", 10389);
        // bind with an username and password
        // this how you can verify the password of an user
        ldapConnection.Bind(dn, password);
        ldapConnection.GetProperty("mail");
        if (!ldapConnection.Bound) throw new UnauthorizedAccessException($"User {username} not find");

        try
        {
            GetUser(dn, ldapConnection);
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
        }

        ldapConnection.Disconnect();
        return await Authenticate(new User(username) { UserName = username }, new CancellationToken(false));
    }


    private void GetUser(string dn, ILdapConnection ldapConnection)
    {
        var entry = ldapConnection.Read(dn);
        Console.Out.WriteLine("User : " + entry.Dn);
        Console.Out.WriteLine("Atributes: ");
        // Parse through the attribute set to get the attributes and the corresponding values
        foreach (var attribute in entry.GetAttributeSet())
        {
            Console.WriteLine($"  {attribute.Name}: {attribute.StringValue}");
        }
    }
}