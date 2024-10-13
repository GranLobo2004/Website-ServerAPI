using FastEndpoints;
using FluentValidation;
using ServerAPI.Data;
using ServerAPI.Features.Auth;

namespace ServerAPI.Features.Updates;

public class UpdateUser:Endpoint<UserRequest,UserResponse>
{
    private readonly DataBase _context;

    public UpdateUser(DataBase context)
    {
        _context = context;
    }
    public override void Configure()
    {
        Post("/update/user");
        AllowAnonymous();
        Validator<UpdateUserValidator>();
    }
    public override async Task HandleAsync(UserRequest req, CancellationToken ct)
    {
        try
        {
            var user = req.User;
            user.LastActivity=DateTime.Now;
            _context.Users.Update(user);
            await _context.SaveChangesAsync(ct);
            await SendAsync(new UserResponse(true, "Usuario registrado correctamente"), 200, ct);
        }
        catch (Exception e)
        {
            await SendAsync(new UserResponse(false, "A problem was found, check the data you provided"), 400, ct);
        }
    }
    internal class UpdateUserValidator : Validator<UserRequest>
    {
        public UpdateUserValidator()
        {
            RuleFor(u => u.User.Name)
                .NotEmpty()
                .WithMessage("Name required for the user")
                .NotNull()
                .WithMessage("Name required");

            RuleFor(u => u.User.Email)
                .NotEmpty()
                .WithMessage("Email required for the user")
                .NotNull()
                .WithMessage("Email required")
                .EmailAddress()
                .WithMessage("Invalid email format");

            RuleFor(u => u.User.Password)
                .NotEmpty()
                .WithMessage("Password required")
                .NotNull()
                .WithMessage("Password required")
                .MinimumLength(8)
                .WithMessage("Password must be at least 8 characters");
            RuleFor(u => u.User.Type)
                .NotEmpty()
                .WithMessage("User type required")
                .NotNull()
                .WithMessage("User type required")
                .Must(type => new[] { "customer", "moderator", "admin" }.Contains(type))
                .WithMessage("Invalid user type.");
            RuleFor(u => u.User.Image)
                .NotEmpty()
                .NotNull();
        }
    }
}