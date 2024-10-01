using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ServerAPI.Data;

namespace ServerAPI.Features.Auth;

public class GetUsers:EndpointWithoutRequest<List<GetUserResponse>>
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
            var response = users.Select(u => new GetUserResponse
            {
                User = u
            }).ToList();
            
            await SendAsync(response, 200, ct);

        }
        catch (Exception e)
        {
            await SendErrorsAsync(500, ct);
        }
    }
}