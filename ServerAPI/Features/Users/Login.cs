using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ServerAPI.Data;
using ServerAPI.Entities;

namespace ServerAPI.Features.Auth;

public record LoginRequest(string Email_Username, string Password);

public record LoginResponse(string Message, User User);

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
        Console.WriteLine("Login Request");
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == req.Email_Username || u.Username == req.Email_Username,
            ct);
        if (user == null)
        {
            await SendAsync(new LoginResponse("Account not found", null), 404, ct);
        }
        
        if (BCrypt.Net.BCrypt.Verify(req.Password, user.Password))
        {
            user.LastActivity = DateTime.Now;
            _context.Users.Update(user);
            Console.WriteLine("Successfully logged in");
            await SendAsync(new LoginResponse("Log in successful", user), 200, ct);
        }
        else
        {
            await SendAsync(new LoginResponse("Password incorrect", default), 400, ct);
        }
    }
}

public class LoginValidator : Validator<LoginRequest>
{
    public LoginValidator()
    {
        RuleFor(r => r.Email_Username)
            .NotEmpty()
            .NotNull()
            .NotEqual("")
            .NotEqual("=")
            .WithMessage("Wrong data to find user");
        RuleFor(r=>r.Password)
            .NotEmpty()
            .NotNull()
            .NotEqual("=")
            .WithMessage("Wrong password");
    }
}
