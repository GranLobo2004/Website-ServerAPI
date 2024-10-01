using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ServerAPI.Data;

namespace ServerAPI.Features
{

    public class GetProducts : EndpointWithoutRequest<List<GetProductResponse>>
    {
        private readonly DataBase _context;

        public GetProducts(DataBase context)
        {
            _context = context;
        }

        public override void Configure()
        {
            Get("/products");
            AllowAnonymous(); // Cambia esto según tu necesidad de autenticación
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            try
            {
                var products = await _context.Products.ToListAsync(ct);
                var response = products.Select(p => new GetProductResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Image = p.Image,
                    Tags = p.Tags,
                }).ToList();

                await SendAsync(response, 200, ct);
            }
            catch (Exception ex)
            {
                await SendErrorsAsync(500, ct);
            }
        }
    }
}
