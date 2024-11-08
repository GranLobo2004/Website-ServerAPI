using FastEndpoints;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using ServerAPI.Data;
using ServerAPI.Entities;
using BCrypt.Net;

namespace ServerAPI.Features.Auth
{
    public record UserRequest(User User);

    public record UserResponse(bool Variable, string Message);

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
            user.Id = 0; // Aseguramos que el ID sea 0 al crear un nuevo usuario
            user.Type = "Customer"; // Asignar tipo por defecto
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            // Verificar si el email ya está registrado
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == user.Email);
            if (existingUser != null)
            {
                await SendAsync(new UserResponse(false, "Email registered"), 400, ct);
                return;
            }
            existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username);
            if (existingUser != null)
            {
                await SendAsync(new UserResponse(false, "Username registered"), 400, ct);
                return;
            }

            try
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync(ct);
                await SendAsync(new UserResponse(true, "User registered"), 200, ct);
            }
            catch (Exception ex)
            {
                // Enviar un mensaje genérico en caso de error
                await SendAsync(new UserResponse(false, "Error al procesar la solicitud: " + ex.Message), 500, ct);
            }
        }
    }

    internal class UserValidator : Validator<UserRequest>
    {
        public UserValidator()
        {
            RuleFor(u => u.User.Id)
                .Must(id => id == 0)
                .WithMessage("ID should not be provided and will be generated automatically.");

            RuleFor(u => u.User.Name)
                .NotEmpty().WithMessage("Name required for the user");

            RuleFor(u => u.User.Email)
                .NotEmpty().WithMessage("Email required for the user")
                .EmailAddress().WithMessage("Invalid email format");

            RuleFor(u => u.User.Username)
                .NotEmpty().WithMessage("Username required for the user")
                .NotNull().WithMessage("Username can not be null");
            
            RuleFor(u => u.User.Password)
                .NotEmpty().WithMessage("Password required")
                .MinimumLength(8).WithMessage("Password must be at least 8 characters");

            RuleFor(u => u.User.Image)
                .Must(url => string.IsNullOrEmpty(url) || Uri.IsWellFormedUriString(url, UriKind.Absolute))
                .WithMessage("Invalid image URL.");
        }
    }
}
