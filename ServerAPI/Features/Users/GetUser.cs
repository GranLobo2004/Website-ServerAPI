using System.ComponentModel.DataAnnotations;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ServerAPI.Data;
using ServerAPI.Entities;

namespace ServerAPI.Features.Auth;

public class GetUserRequest
{
    public string Email { get; set; }
}

public class GetUserResponse
{
    public User User { get; set; }
}

public class GetUser:Endpoint<GetUserRequest, GetUserResponse>
{
    private readonly DataBase _context;

    public GetUser(DataBase context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Get("/user/{Email}");
        AllowAnonymous();
    }
    public override async Task HandleAsync(GetUserRequest req, CancellationToken ct)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == req.Email, ct);

            if (user == null)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            var response = new GetUserResponse()
            {
                User = user
            };

            await SendAsync(response, 200, ct);
        }
        catch (Exception ex)
        {
            // Manejar el error y enviar una respuesta de error
            await SendErrorsAsync(500, ct);
        }
    }
}
