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
    
    public class GetProductResponse
    {
        public Product Product { get; set; }
    }

    public class GetProduct : EndpointWithoutRequest< GetProductResponse>
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

        public override async Task HandleAsync(CancellationToken ct)
        {
            try
            {
                var id = Route<int>("id");
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == id, ct);
                if (product == null)
                {
                    await SendNotFoundAsync(ct);
                    return;
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
