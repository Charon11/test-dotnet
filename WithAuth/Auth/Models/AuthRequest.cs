namespace WithAuth.Auth.Models;

using FluentValidation;

public class AuthRequest
{
    public string? Username { get; set; }
    public string? Password { get; set; }
}

public class AuthRequestValidator : AbstractValidator<AuthRequest>
{
    public AuthRequestValidator()
    {
        RuleFor(request => request.Username).NotNull();
        RuleFor(request => request.Password).NotNull();
    }
}