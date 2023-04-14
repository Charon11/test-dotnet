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

    public AuthenticateService(IAccessTokenService accessTokenService, IRefreshTokenService refreshTokenService,
        IApplicationDbContext context, UserManager<User> userManager, IRefreshTokenValidator refreshTokenValidator,
        SignInManager<User> signInManager)
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
        using var cn = new LdapConnection();
        // connect
        cn.Connect("127.0.0.1", 10389);
        // bind with an username and password
        // this how you can verify the password of an user
        cn.Bind($"uid={username},ou=users,dc=wimpi,dc=net", password);
        var mail = cn.GetProperty("mail");
        if (!cn.Bound) throw new UnauthorizedAccessException($"User {username} not find");

        return await Authenticate(new User(username) { UserName = username }, new CancellationToken(false));
    }
}