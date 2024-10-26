using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ServerAPI.Data;
using ServerAPI.Entities;

namespace ServerAPI.Features.Auth;

public class GetUsers:EndpointWithoutRequest<List<User>>
{
    private readonly DataBase _context;

    public GetUsers(DataBase context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Get("/users");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        try
        {
            var users = await _context.Users.ToListAsync();
        
            // Enviar directamente la lista de usuarios
            await SendAsync(users, 200, ct);
        }
        catch (Exception e)
        {
            await SendErrorsAsync(500, ct);
        }
    }

}