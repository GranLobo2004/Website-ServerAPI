using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ServerAPI.Data;
using ServerAPI.Entities;

namespace ServerAPI.Features.Auth;

public record LoginRequest(string Email, string Password);

public record LoginResponse(string message, User user);

public class Login : Endpoint<LoginRequest, LoginResponse>
{
    private readonly DataBase _context;
    public Login(DataBase context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Post("/login");
        AllowAnonymous();
        Validator<LoginValidator>();
    }
    
    public override async Task HandleAsync(LoginRequest req, CancellationToken ct)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == req.Email,
                ct);
            if (user.Password == req.Password)
            {
                user.LastActivity = DateTime.Now;
                _context.Users.Update(user);
                await SendAsync(new LoginResponse("Log in successful", user), 200, ct);
            }
            else
            {
                await SendAsync(new LoginResponse("Password incorrect", default), 400, ct);
            }
        }
        catch (Exception ex)
        {
            await SendAsync(new LoginResponse("An error ocurred, check the email and the password.", null), 400, ct);
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
