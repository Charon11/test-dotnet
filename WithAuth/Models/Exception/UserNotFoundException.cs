namespace WithAuth.Models.Exception;

public class UserNotFoundException : System.Exception
{
    private UserNotFoundException() : base("User can't be found.") { }
    public static UserNotFoundException Instance { get; } = new();
}