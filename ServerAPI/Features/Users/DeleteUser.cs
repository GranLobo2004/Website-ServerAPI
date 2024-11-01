using System.Net;
using ServerAPI.Data;
using ServerAPI.Entities;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace ServerAPI.Features.Users;

public record DeleteUserRequest {
    public int Id { get; set; } }

public record DeleteUserResponse ( string Message );

public class DeleteUser: Endpoint<DeleteUserRequest, DeleteUserResponse>
{
    private readonly DataBase _context;

    public DeleteUser(DataBase context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Post("/user/delete");
        AllowAnonymous();
    }

    public override async Task HandleAsync(DeleteUserRequest req, CancellationToken ct)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == req.Id, ct);
            Console.WriteLine(user);
            if (user == null)
            {
                await SendAsync(new DeleteUserResponse("No user found."), 404, ct);
            }
            else
            {
                _context.Users.Remove(user);
                _context.SaveChanges();
                await SendAsync(new DeleteUserResponse("User deleted."), 200, ct);
            }
        }
        catch (Exception ex)
        {
            await SendAsync(new DeleteUserResponse(ex.Message), 400, ct);
        }
    }
    
}