using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ServerAPI.Data;
using ServerAPI.Entities;

namespace ServerAPI.Features.Auth;

public record LoginRequest(string Email, string Password);

public record LoginResponse(bool Success, User user);

public class Login : Endpoint<LoginRequest, LoginResponse>
{
    private readonly DataBase _context;
    public Login(DataBase context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Post("/login/{Email}");
        AllowAnonymous();
        Validator<LoginValidator>();
    }
    
    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == req.Email & u.Password == req.Password,
                ct);
            await SendAsync(new LoginResponse(true, user), 200, ct);
        }
        catch (Exception ex)
        {
            await SendAsync(new LoginResponse(false, null), 400, ct);
        }
    }
}

public class LoginValidator : Validator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(r => r.Email)
            .NotEmpty()
            .NotNull()
            .NotEqual("")
            .NotEqual("=")
            .EmailAddress()
            .WithMessage("Wrong format email");
        RuleFor(r=>r.Password)
            .NotEmpty()
            .NotNull()
            .NotEqual("=")
            .WithMessage("Wrong password");
    }
}
