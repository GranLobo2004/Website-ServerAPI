using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ServerAPI.Data;
using ServerAPI.Entities;

namespace ServerAPI.Features;

public class GetComments : EndpointWithoutRequest<List<Comment>>
{
    private readonly DataBase _context;

    public GetComments(DataBase context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Get("/comments/{id}");
        AllowAnonymous(); // Cambia esto según tu necesidad de autenticación
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var id = Route<int>("id");
        try
        {
            var  comments= await _context.Comments.Where(c => c.ProductId == id).ToListAsync();

            if (comments.Count == 0)
            {
                await SendAsync([], 200, ct);
            }

            List<Comment> response = comments.AsEnumerable().Reverse().ToList();
            await SendAsync(response, 200, ct);
        }
        catch (Exception ex)
        {
            // Manejar el error y enviar una respuesta de error
            await SendErrorsAsync(500, ct);
        }
    }
}