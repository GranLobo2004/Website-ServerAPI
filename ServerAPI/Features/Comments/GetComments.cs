using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ServerAPI.Data;
using ServerAPI.Entities;

namespace ServerAPI.Features;

public class GetCommentsRequest
{
    public int ProductId { get; set; }
}

public class GetComments : Endpoint<GetCommentsRequest, List<Comment>>
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

    public async Task HandleAsync(GetProductRequest req, CancellationToken ct)
    {
        try
        {
            var comments = await _context.Comments
                .Where(c => c.ProductId == req.Id)
                .ToListAsync(ct);

            // Verifica si la lista está vacía
            if (comments == null || !comments.Any())
            {
                await SendNotFoundAsync(ct);
                return;
            }

            await SendAsync(comments, 200, ct);
        }
        catch (Exception ex)
        {
            // Manejar el error y enviar una respuesta de error
            await SendErrorsAsync(500, ct);
        }
    }
}