using System.Net;
using ServerAPI.Data;
using ServerAPI.Entities;
using FastEndpoints;
using Microsoft.EntityFrameworkCore;

namespace ServerAPI.Features.Users;


public record DeleteProductResponse ( string Message );

public class DeleteProduct: EndpointWithoutRequest<DeleteProductResponse>
{
    private readonly DataBase _context;

    public DeleteProduct(DataBase context)
    {
        _context = context;
    }

    public override void Configure()
    {
        Get("/product/delete/{id}");
        AllowAnonymous();
    }

    public override async Task HandleAsync( CancellationToken ct)
    {
        try
        {
            var id = Route<int>("id");
            var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id, ct);
            if (product == null)
            {
                await SendAsync(new DeleteProductResponse("Product not found."), 404, ct);
            }
            else
            {
                var comments = await _context.Comments.Where(c => c.ProductId == product.Id).ToListAsync();
                for (int i = 0; i < comments.Count; i++)
                {
                    _context.Comments.Remove(comments[i]);
                }
                _context.Products.Remove(product);
                _context.SaveChanges();
                await SendAsync(new DeleteProductResponse("Product deleted."), 200, ct);
            }
        }
        catch (Exception ex)
        {
            await SendAsync(new DeleteProductResponse(ex.Message), 400, ct);
        }
    }
    
}