using ServerAPI.Entities;
using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ServerAPI.Data;

namespace ServerAPI.Features
{
    internal record UserRequest(User User);

    internal record UserResponse(bool Variable, string Message);

    internal sealed class CheckUser : Endpoint<UserRequest, UserResponse>
    {
        private readonly DataBase _context;

        public CheckUser(DataBase context)
        {
            _context = context;
        }

        public override void Configure()
        {
            Post("/user");
            AllowAnonymous();
            Validator<UserValidator>();
        }

        public override async Task HandleAsync(UserRequest req, CancellationToken ct)
        {
            var user = req.User;
            user.Id = 0;
            _context.Users.Add(user);
            await _context.SaveChangesAsync(ct);
            await SendAsync(new UserResponse(true, "Usuario registrado correctamente"), 200, ct);
        }
    }

    internal class UserValidator : Validator<UserRequest>
    {
        public UserValidator()
        {
            RuleFor(u => u.User.Id)
                .Equal(0)
                .WithMessage("ID should not be provided and will be generated automatically.");

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
        }
    }
}
