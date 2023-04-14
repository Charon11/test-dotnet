namespace WithAuth.Models.Exception;

public class InvalidRefreshTokenException : System.Exception
{
    private InvalidRefreshTokenException() : base("Refresh token is not valid.") { }
    public static InvalidRefreshTokenException Instance { get; } = new();
}