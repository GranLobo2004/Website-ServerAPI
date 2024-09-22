using FastEndpoints;
using Microsoft.EntityFrameworkCore;
using ServerAPI.Data;

namespace ServerAPI.Features
{
    public class GetProductsResponse
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        
        public string Image { get; set; }
        
        public List<string> Tags { get; set; }
    }
    
    public class GetProducts : EndpointWithoutRequest<List<GetProductsResponse>>
    {
        private readonly DataBase _context;

        public GetProducts(DataBase context)
        {
            _context = context;
        }

        public override void Configure()
        {
            Get("/product");
            AllowAnonymous(); // Cambia esto según tu necesidad de autenticación
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            try
            {
                var products = await _context.Products.ToListAsync(ct);
                var response = products.Select(p => new GetProductsResponse
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    Image = p.Image,
                    Tags = p.Tags
                }).ToList();

                await SendAsync(response, 200, ct);
            }
            catch (Exception ex)
            {
                // Manejar el error y enviar una respuesta de error
            }
            
        }
    }
}