using ServerAPI.Entities;

namespace ServerAPI.Features
{
    using FastEndpoints;
    using Microsoft.EntityFrameworkCore;
    using ServerAPI.Data;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;

    public class GetProductRequest
    {
        public int Id { get; set; }
    }

    public class CommentResponse
    {
        public Comment Comment { get; set; }
    }
    
    public class GetProductResponse
    {
        public Product Product { get; set; }
    }

    public class GetProduct : Endpoint<GetProductRequest, GetProductResponse>
    {
        private readonly DataBase _context;

        public GetProduct(DataBase context)
        {
            _context = context;
        }

        public override void Configure()
        {
            Get("/product/{id}");
            AllowAnonymous(); // Cambia esto según tu necesidad de autenticación
        }

        public override async Task HandleAsync(GetProductRequest req, CancellationToken ct)
        {
            try
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == req.Id, ct);
                var comments = await _context.Comments.Where(c => c.ProductId == product.Id).ToListAsync(ct);
                if (product == null)
                {
                    await SendNotFoundAsync(ct);
                    return;
                }

                if (comments == null)
                {
                    product.Comments = new List<Comment>();
                }
                else
                {
                    product.Comments = comments;
                }
                
                var response = new GetProductResponse()
                {
                    Product = product
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
}
